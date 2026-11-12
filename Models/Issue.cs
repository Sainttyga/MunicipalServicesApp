namespace MunicipalServicesApp.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? AttachmentPath { get; set; }
        public DateTime ReportedDate { get; set; }
        public string Status { get; set; } = "Pending";

        public Issue()
        {
            ReportedDate = DateTime.Now;
        }
    }

    public static class IssueDataStorage
    {
        private static readonly string StorageFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, 
            "issues_data.json"
        );

        public static List<Issue> LoadIssues()
        {
            try
            {
                if (File.Exists(StorageFilePath))
                {
                    string json = File.ReadAllText(StorageFilePath);
                    var issues = System.Text.Json.JsonSerializer.Deserialize<List<Issue>>(json);
                    return issues ?? new List<Issue>();
                }
            }
            catch (Exception)
            {
                // If loading fails, return empty list
            }
            return new List<Issue>();
        }

        public static void SaveIssues(List<Issue> issues)
        {
            try
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = System.Text.Json.JsonSerializer.Serialize(issues, options);
                File.WriteAllText(StorageFilePath, json);
            }
            catch (Exception)
            {
                // Handle save error silently or log it
            }
        }

        public static int GetNextIssueId(List<Issue> issues)
        {
            if (issues.Count == 0)
                return 1;
            return issues.Max(i => i.Id) + 1;
        }
    }
}
