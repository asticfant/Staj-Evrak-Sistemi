namespace StajEvrakSistemi.Models;

public class Admin : User
{
    public Admin()
    {
        Rol = UserRole.Admin;
    }
}