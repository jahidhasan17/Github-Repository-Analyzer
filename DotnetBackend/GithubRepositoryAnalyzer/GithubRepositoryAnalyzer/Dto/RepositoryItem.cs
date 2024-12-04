namespace GithubRepositoryAnalyzer.Dto;

public class RepositoryItem
{
    public string Name { get; set; }

    public string UserName { get; set; }
    
    public string ForkName { get; set; }
    
    public string Description { get; set; }
    
    public string Star { get; set; }
    
    public string Fork { get; set; }
    
    public string ModifiedDate { get; set; }
    
    public string CreatedDate { get; set; }

    public List<RepositoryLanguage> Languages { get; set; }
    
    public string MainBranchName { get; set; }
}