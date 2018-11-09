namespace Orleans.Storage.EntityFrameworkCore
{
    public interface IStorageEntity
    {
        int VersionNo { get; set; }
    }
}
