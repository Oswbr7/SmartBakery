using SmartBakery.Domain.Common;

namespace SmartBakery.Domain.Entities;

public sealed class User : Entity, IHasUpdatedAt
{
    public const int UsernameMaxLength = 100;
    public const int EmailMaxLength = 255;

    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    // El dominio solo conoce el hash. Generarlo (BCrypt, PBKDF2...) es un
    // detalle técnico que implementaremos en Infrastructure en la Fase 2.
    public string PasswordHash { get; private set; } = string.Empty;

    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private User() { }

    public static User Create(string username, string email, string passwordHash, Guid roleId) => new()
    {
        Username = Guard.AgainstNullOrWhiteSpace(username, nameof(Username), UsernameMaxLength),
        Email = NormalizeEmail(email),
        PasswordHash = Guard.AgainstNullOrWhiteSpace(passwordHash, nameof(PasswordHash)),
        RoleId = Guard.AgainstEmpty(roleId, nameof(RoleId)),
        IsActive = true
    };

    public void ChangeRole(Guid roleId) => RoleId = Guard.AgainstEmpty(roleId, nameof(RoleId));

    public void ChangePasswordHash(string passwordHash) =>
        PasswordHash = Guard.AgainstNullOrWhiteSpace(passwordHash, nameof(PasswordHash));

    public void Activate() => IsActive = true;

    // Soft delete: un usuario con ventas registradas nunca se borra físicamente.
    public void Deactivate() => IsActive = false;

    // Guardar el email en minúsculas evita duplicados como "Ana@x.com" y "ana@x.com".
    // La validación completa del formato la hará FluentValidation en Application.
    private static string NormalizeEmail(string email)
    {
        var normalized = Guard.AgainstNullOrWhiteSpace(email, nameof(Email), EmailMaxLength).ToLowerInvariant();

        if (!normalized.Contains('@'))
            throw new DomainException("Email is not valid.");

        return normalized;
    }
}
