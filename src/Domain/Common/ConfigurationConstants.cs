namespace Nuyken.VeGasCo.Backend.Domain.Common;

public static class ConfigurationConstants
{
    public const string SWAGGER_TITLE_KEY = "Swagger:Title";
    public const string SWAGGER_DESCRIPTION_KEY = "Swagger:Description";
    public const string JWT_AUTHORITY_KEY = "Jwt:Authority";
    public const string JWT_ISSUER_KEY = "Jwt:Issuer";
    public const string JWT_AUDIENCE_KEY = "Jwt:Audience";
    public const string JWT_ENABLE_ROLE_BASED_AUTHORIZATION = "Jwt:EnableRoleBasedAuthorization";
    public const string JWT_ROLE_KEY = "Jwt:RoleKey";
    public const string JWT_ROLE_VALUE = "Jwt:RoleValue";
    public const string DATABASE_AUTOMIGRATE = "Database:AutoMigrate";
    public const string DATABASE_TYPE = "Database:Type";
    public const string CONNECTION_STRING_NAME = "Default";
    public const string DEFAULT_CONNECTION_STRING = "ConnectionStrings:Default";
    public const string DEFAULT_CONNECTION_STRING_ENV_NAME = "CONNECTION_STRING";
}