namespace MediSenior.Services;

public static class FirebaseConfig
{
    public const string ApiKey = "YOUR_FIREBASE_API_KEY";

    public const string ProjectId = "YOUR_FIREBASE_PROJECT_ID";

    public static string FirestoreBaseUrl =>
        $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents";
}
