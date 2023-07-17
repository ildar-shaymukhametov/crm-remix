namespace CRM.Domain;

public static class Constants
{
    public static class Claims
    {
        public const string ClaimType = "authorization";

        public static class Company
        {
            public const string Create = $"{nameof(Company)}.{nameof(Create)}";

            public static class New
            {
                public static class Other
                {
                    public const string Set = $"{nameof(Company)}.{nameof(New)}.{nameof(Other)}.{nameof(Set)}";
                }
                public static class Manager
                {
                    public const string SetToAny = $"{nameof(Company)}.{nameof(New)}.{nameof(Manager)}.{nameof(SetToAny)}";
                    public const string SetToSelf = $"{nameof(Company)}.{nameof(New)}.{nameof(Manager)}.{nameof(SetToSelf)}";
                    public const string SetToNone = $"{nameof(Company)}.{nameof(New)}.{nameof(Manager)}.{nameof(SetToNone)}";
                }
            }

            public static class Any
            {
                public const string Delete = $"{nameof(Company)}.{nameof(Any)}.{nameof(Delete)}";

                public static class Manager
                {
                    public const string Get = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(Get)}";
                    public const string SetFromAnyToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToAny)}";
                    public const string SetFromAnyToSelf = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToSelf)}";
                    public const string SetFromNoneToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromNoneToAny)}";
                    public const string SetFromNoneToSelf = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromNoneToSelf)}";
                    public const string SetFromAnyToNone = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToNone)}";
                    public const string SetFromSelfToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromSelfToAny)}";
                    public const string SetFromSelfToNone = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromSelfToNone)}";
                }
                public static class Other
                {
                    public const string Get = $"{nameof(Company)}.{nameof(Any)}.{nameof(Other)}.{nameof(Get)}";
                    public const string Set = $"{nameof(Company)}.{nameof(Any)}.{nameof(Other)}.{nameof(Set)}";
                }
                public static class Name
                {
                    public const string Get = $"{nameof(Company)}.{nameof(Any)}.{nameof(Name)}.{nameof(Get)}";
                    public const string Set = $"{nameof(Company)}.{nameof(Any)}.{nameof(Name)}.{nameof(Set)}";
                }
            }

            public static class WhereUserIsManager
            {
                public const string Delete = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Delete)}";

                public static class Other
                {
                    public const string Get = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Other)}.{nameof(Get)}";
                    public const string Set = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Other)}.{nameof(Set)}";
                }
                public static class Manager
                {
                    public const string Get = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(Get)}";
                    public const string SetFromSelfToAny = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(SetFromSelfToAny)}";
                    public const string SetFromSelfToNone = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(SetFromSelfToNone)}";
                }
                public static class Name
                {
                    public const string Get = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Name)}.{nameof(Get)}";
                    public const string Set = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Name)}.{nameof(Set)}";
                }
            }
        }
    }

    public static class Access
    {
        public static class Company
        {
            public const string Create = $"{nameof(Company)}.{nameof(Create)}";

            public static class New
            {
                public static class Other
                {
                    public const string Set = $"{nameof(Company)}.{nameof(New)}.{nameof(Other)}.{nameof(Set)}";
                }
                public static class Manager
                {
                    public const string SetToAny = $"{nameof(Company)}.{nameof(New)}.{nameof(Manager)}.{nameof(SetToAny)}";
                    public const string SetToSelf = $"{nameof(Company)}.{nameof(New)}.{nameof(Manager)}.{nameof(SetToSelf)}";
                    public const string SetToNone = $"{nameof(Company)}.{nameof(New)}.{nameof(Manager)}.{nameof(SetToNone)}";
                }
            }

            public static class Any
            {
                public const string Delete = $"{nameof(Company)}.{nameof(Any)}.{nameof(Delete)}";

                public static class Manager
                {
                    public const string Get = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(Get)}";
                    public const string SetFromAnyToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToAny)}";
                    public const string SetFromAnyToSelf = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToSelf)}";
                    public const string SetFromNoneToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromNoneToAny)}";
                    public const string SetFromNoneToSelf = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromNoneToSelf)}";
                    public const string SetFromAnyToNone = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToNone)}";
                    public const string SetFromSelfToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromSelfToAny)}";
                    public const string SetFromSelfToNone = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromSelfToNone)}";
                }
                public static class Other
                {
                    public const string Get = $"{nameof(Company)}.{nameof(Any)}.{nameof(Other)}.{nameof(Get)}";
                    public const string Set = $"{nameof(Company)}.{nameof(Any)}.{nameof(Other)}.{nameof(Set)}";
                }
                public static class Name
                {
                    public const string Get = $"{nameof(Company)}.{nameof(Any)}.{nameof(Name)}.{nameof(Get)}";
                    public const string Set = $"{nameof(Company)}.{nameof(Any)}.{nameof(Name)}.{nameof(Set)}";
                }
            }

            public static class WhereUserIsManager
            {
                public const string Delete = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Delete)}";

                public static class Other
                {
                    public const string Get = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Other)}.{nameof(Get)}";
                    public const string Set = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Other)}.{nameof(Set)}";
                }
                public static class Manager
                {
                    public const string Get = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(Get)}";
                    public const string SetFromSelfToAny = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(SetFromSelfToAny)}";
                    public const string SetFromSelfToNone = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(SetFromSelfToNone)}";
                }
                public static class Name
                {
                    public const string Get = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Name)}.{nameof(Get)}";
                    public const string Set = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Name)}.{nameof(Set)}";
                }
            }
        }
    }

    public static class Roles
    {
        public const string Administrator = "Administrator";
    }
}