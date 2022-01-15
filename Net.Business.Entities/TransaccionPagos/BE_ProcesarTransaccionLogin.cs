namespace Net.Business.Entities
{
    public class BE_ProcesarTransaccionLoginRequest
    {
        public BE_ProcesarTransaccionLoginRequest(string _ecr_usuario,string _ecr_password)
        {
            this.ecr_usuario = _ecr_usuario;
            this.ecr_password = _ecr_password;
        }
        public string ecr_usuario { get; set; }
        public string ecr_password { get; set; }

    }
    public class BE_ProcesarTransaccionLoginResponse
    {
        public string token { get; set; }
        public string resultado { get; set; }
        public string mensaje { get; set; }

    }
}
