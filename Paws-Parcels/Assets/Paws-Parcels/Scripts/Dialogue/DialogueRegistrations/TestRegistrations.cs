using Yarn.Unity;

namespace NPCDialogs
{
    public class TestRegistrations : BaseNPCRegistration
    {
        protected override void Bind()
        {
            RegisterCommand("command_name", runner => runner.AddCommandHandler<string, int>("command_name", TestCommand));
            RegisterFunction("function_name", runner => runner.AddFunction<string, bool>("function_name", TestFunction));
        }

        private void TestCommand(string testString, int testInt)
        {
            print(testString + testInt);
        }

        private bool TestFunction(string testString)
        {
            print(testString);
            return true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            //что-то свое
        }

        protected override void Start()
        {
            base.Start();
            
            //что-то свое
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            //что-то свое
        }
    }
}