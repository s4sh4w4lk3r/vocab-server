namespace Vocab.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
// https://private-user-images.githubusercontent.com/47791892/247092894-39b3d800-fecf-4c5a-b702-9f767ee03169.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3MTI0MTQwMjMsIm5iZiI6MTcxMjQxMzcyMywicGF0aCI6Ii80Nzc5MTg5Mi8yNDcwOTI4OTQtMzliM2Q4MDAtZmVjZi00YzVhLWI3MDItOWY3NjdlZTAzMTY5LnBuZz9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFWQ09EWUxTQTUzUFFLNFpBJTJGMjAyNDA0MDYlMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjQwNDA2VDE0Mjg0M1omWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPWQyMDBhZTAwOTY2ZTRlYjI1OWQ2MDhiYzc4YzE2NmJmM2M5N2E4MDVmYmUyZWViZGNiYjc0MjEwMzgzZjdjNjQmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0JmFjdG9yX2lkPTAma2V5X2lkPTAmcmVwb19pZD0wIn0.OPTTtGec03mLscc4g8Z9V19jP8LLvjILisVExVLcXwE