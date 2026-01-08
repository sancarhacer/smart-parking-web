using Google.Cloud.Firestore;

public class FirebaseService
{
    private readonly FirestoreDb _db;

    public FirebaseService(IConfiguration config)
    {
        Environment.SetEnvironmentVariable(
            "GOOGLE_APPLICATION_CREDENTIALS",
            config["Firebase:CredentialPath"]
        );

        _db = FirestoreDb.Create(config["Firebase:ProjectId"]);
    }

    public async Task<List<string>> GetAllParkingsAsync()
    {
        var snapshot = await _db.Collection("otoparklar").GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.Id).ToList();
    }
}
