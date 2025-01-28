namespace Core;

public class CharityCaseSpecParams
{

    private const int MaxPageSize = 30;
    public int PageIndex {get; set;} = 1;
    private int _pageSize = 6;
    public int PageSize
    {
      get=> _pageSize;
      set => _pageSize =(value > MaxPageSize) ? MaxPageSize : value;
    }
    

    private List<string> _categories = [];
    public List<string> Categories
    {
      get => _categories;
      set {
        _categories = value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();  
      }
    }

     private List<string> _levels = [];
    public List<string> Levels
    {
      get => _levels;
      set {
        _levels = value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();  
      }
    } 
    public string?  Sort { get; set; }    
    private string? _search
    ;
    public string? Search
    {
      get => _search ?? "";
      set => _search = value.ToLower();
    }
    
    
}
