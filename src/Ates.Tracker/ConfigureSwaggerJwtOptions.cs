using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ates.Tracker;

public class ConfigureSwaggerJwtOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()  
        {  
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        });
    
        options.AddSecurityRequirement(new OpenApiSecurityRequirement  
        {  
            {  
                new OpenApiSecurityScheme
                {  
                    Reference = new OpenApiReference
                    {  
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }  
                },
                Array.Empty<string>()
            }  
        });  
    }
}