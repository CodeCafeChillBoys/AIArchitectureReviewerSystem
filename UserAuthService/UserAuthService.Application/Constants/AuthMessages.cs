namespace UserAuthService.Application.Constants;

public static class AuthMessages
{
    public const string OperationSuccess = "Operation completed successfully.";
    public const string LoginSuccess = "Login successful.";
    public const string InvalidCredentials = "Invalid email or password.";
    public const string UserAlreadyExists = "User with this email already exists.";
    public const string RegisterSuccess = "User registered successfully.";
    public const string RegisterFailed = "Registration failed.";
    public const string ValidationFailed = "Validation failed.";
    public const string InvalidGoogleToken = "Invalid Google token.";
    public const string GoogleEmailNotFound = "Email not found in Google token.";
}
