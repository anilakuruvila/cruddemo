namespace Assessment.Api.Utilities
{
    public static class ErrorMessages
    {
        public const string SearchedPersonNotFound= "Searched person not found for Id ";
        public const string PersonNameIsUnique = "Person name already taken. Please choose a different name";
        public const string PersonNameIsRequired = "Person Name should be entered";
        public const string PersonAddressIsRequired= "Person Address should be entered";
        public const string PersonNameNotExceed50 = "Person Name should not exceed 50 characters";
        public const string PersonAddressNotExceed250 = "Person Address should not exceed 50 characters";

    }
}
