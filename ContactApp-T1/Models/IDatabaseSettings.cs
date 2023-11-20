namespace ContactApp_T1.Models
{
    public interface IDatabaseSettings
    {
        public string UsersCollectionName { get; set; } 
        public string ConnectionString {  get; set; }   
        public string DatabaseName { get; set; }    
    }
}
