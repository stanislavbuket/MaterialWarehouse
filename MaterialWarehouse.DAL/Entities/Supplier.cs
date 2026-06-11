namespace MaterialWarehouse.DAL.Entities;

public class Supplier
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Contacts { get; private set; } = string.Empty;

    protected Supplier() { }

    public Supplier(int id, string name, string contacts)
    {
        Id = id;
        Name = name;
        Contacts = contacts;
    }
}