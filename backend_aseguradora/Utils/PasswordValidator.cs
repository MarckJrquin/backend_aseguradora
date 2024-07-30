using System.Text.RegularExpressions;

namespace backend_aseguradora.Utils
{
    public class PasswordValidator
    {
        public static bool IsValid(string password)
        {
            // Expresión regular para validar que la contraseña tenga al menos una letra, un número y una mayúscula.
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$");
            return regex.IsMatch(password);
        }
    }
}
