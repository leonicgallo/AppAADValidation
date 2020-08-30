using APIDoctores.Extensions;
using APIDoctores.Models;
using APIDoctores.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace APIDoctores.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly AzureAdOptions _azureOptions;

        public UsersController(IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
        {
            _azureOptions = new AzureAdOptions();
            configuration.Bind("AzureAd", _azureOptions);
            _graphServiceClientFactory = graphServiceClientFactory;
        }

        //[Authorize]
        [HttpGet]
        public IEnumerable<Doctor> Get()
        {
            var bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var doctoresList = new List<Doctor>();

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(bearer_token);
                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient();
                var tenantIssuer = _azureOptions.TenantIssuer;

                if (token.Issuer.Contains(tenantIssuer))
                {
                    var doctores = GraphService.GetUsersList(graphClient);

                    foreach (var x in doctores.Result)
                    {
                        var doc = new Doctor
                        {
                            Telefonos = x.BusinessPhones,
                            NombreCompleto = x.DisplayName,
                            Nombre = x.GivenName,
                            Apellido = x.Surname,
                            Email = x.Mail,
                            DoctorId = x.Id,
                            Titulo = x.JobTitle,
                            Celular = x.MobilePhone
                        };

                        doctoresList.Add(doc);
                    }

                    return doctoresList;
                }
            }catch(Exception ex)
            {
                return null;
            }

            return null;
        }
    }
}
