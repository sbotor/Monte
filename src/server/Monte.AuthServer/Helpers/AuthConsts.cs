﻿namespace Monte.AuthServer.Helpers;

internal static class AuthConsts
{
    public static class Scopes
    {
        public const string MonteMainApi = "monte_main_api";
        public const string MonteAgentApi = "monte_agent_api";
    }

    public static class Roles
    {
        public const string MonteAgent = "monte_agent";
        public const string MonteAdmin = "monte_admin";
        public const string MonteUser = "monte_user";

    }

    public static class RoleGroups
    {
        public const string AllUsers = Roles.MonteAdmin + "," + Roles.MonteUser;
    }

    public static class Claims
    {
        public const string IsExternal = "is_external";
    }
}
