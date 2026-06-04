namespace MaterialWarehouse.DAL.Entities;

public class Category
{
    private readonly List<Material> _materials = [];

    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public IReadOnlyCollection<Material> Materials => _materials.AsReadOnly();

    protected Category() { }

    public Category(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public void AddMaterial(Material material)
    {
        ArgumentNullException.ThrowIfNull(material);
        _materials.Add(material);
    }
}
