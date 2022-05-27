using System.Reflection;

namespace ADSD.Backend.App.Helpers;

public class ClassesAndMethodsExtractor
{
    public void GetAllClassesAndMethodsOfAssembly()
    {
        //Code to load Assembly

        var assem1 = Assembly.GetAssembly(GetType());


        //Another Way

        var assem2 = Assembly.Load("ADSD.Backend.App");


        //Get List of Class Name

        var types = assem1.GetTypes();


        foreach (var tc in types.Where(x => x.Namespace?.Contains("ADSD.Backend.App") == true))

        {
            if (tc.IsAbstract)

            {
                Console.WriteLine("Abstract Class : " + tc.Name);
            }

            else if (tc.IsPublic)

            {
                Console.WriteLine("Public Class : " + tc.Name);
            }

            else if (tc.IsSealed)

            {
                Console.WriteLine("Sealed Class : " + tc.Name);
            }


            //Get List of Method Names of Class

            var methodName = tc.GetMethods();


            foreach (var method in methodName)
            {
                if (!method.IsFamilyAndAssembly)
                {
                    continue;
                }
                
                if (method.ReflectedType is {IsPublic: true})

                {
                    Console.WriteLine("Public Method : " + method.Name);
                }

                else

                {
                    Console.WriteLine("Non-Public Method : " + method.Name);
                }
            }
        }
    }
}