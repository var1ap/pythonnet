using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Loader;
using Python.Runtime;

namespace Python.Runtime
{
    public sealed class PythonConsole
    {
        private PythonConsole()
        {
        }

        [STAThread]
        public static int Main(string[] args)
        {
            // reference the static assemblyLoader to stop it being optimized away
            AssemblyLoader a = assemblyLoader;

            string[] cmd = Environment.GetCommandLineArgs();
            //PythonEngine.Initialize();
        IntPtr gs;

        PythonEngine.Initialize();
            gs = PythonEngine.AcquireLock();

            PyList list = new PyList();
            list.Append(new PyString("foo"));
            list.Append(new PyString("bar"));
            list.Append(new PyString("baz"));
            List<string> result = new List<string>();
            foreach (PyObject item in list)
                result.Add(item.ToString());
            Debug.Assert(3 == result.Count);
            Debug.Assert("foo" == result[0]);
            Debug.Assert("bar" == result[1]);
            Debug.Assert("baz" == result[2]);

            PythonEngine.ReleaseLock(gs);
            int i = Runtime.Py_Main(cmd.Length, cmd);
            //PythonEngine.Shutdown();
            PythonEngine.Shutdown();

            return i;
        }

        // Register a callback function to load embedded assmeblies.
        // (Python.Runtime.dll is included as a resource)
        private sealed class AssemblyLoader
        {
            Dictionary<string, Assembly> loadedAssemblies;

            public AssemblyLoader()
            {
                loadedAssemblies = new Dictionary<string, Assembly>();

                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    string shortName = args.Name.Split(',')[0];
                    String resourceName = shortName + ".dll";

                    if (loadedAssemblies.ContainsKey(resourceName))
                    {
                        return loadedAssemblies[resourceName];
                    }

                    // looks for the assembly from the resources and load it
                    using (var stream = Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
                            loadedAssemblies[resourceName] = assembly;
                            return assembly;
                        }
                    }

                    return null;
                };
            }
        };

        private static AssemblyLoader assemblyLoader = new AssemblyLoader();
    };
}