using System;
using System.Collections.Generic;
using System.Reflection;
using Transport.CommandsBase;

namespace ClientExtensionsBase
{
    public sealed class CommandContainer<T>
    {
        public T CommandRef { get; set; }
        public Type CommandType { get; set; }
    }

    public static class ReflectionHelper
    {
        public static Dictionary<string,IServerCommand> GetAllCommandsInAssembly()
        {
            var alltypes = Assembly.GetExecutingAssembly().GetTypes();
            var commands = new Dictionary<string, IServerCommand>(1000);
            foreach (var type in alltypes)
            {

                if (type.IsAssignableFrom(typeof (IServerCommand)))
                {
                    var instance = Activator.CreateInstance(type) as IServerCommand;
                    commands.Add(type.Name, instance);
                }
                 
            }

            return commands;
        }
    }
}