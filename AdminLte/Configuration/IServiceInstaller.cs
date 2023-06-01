namespace AdminLte.Configuration
{
    public interface IServiceInstaller
    {
        void Install(IServiceCollection services,IConfiguration configuration);

        public int order => -1;
    }
}
