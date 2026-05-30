namespace MediSenior.Services;

public static class FirebaseConfig
{
    public const string ApiKey = "AIzaSyCEbxxlCPA5xNCJ4N-4OVg1264lMPtFltM";

    public const string ProjectId = "medisenior-de333";

    public static string FirestoreBaseUrl =>
        $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents";
}
