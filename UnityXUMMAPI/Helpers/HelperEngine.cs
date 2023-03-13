using System;
using SharedLibrary;
using XUMM.NET.SDK.Models.Payload;

namespace UnityXUMMAPI.Helpers
{
    public interface IHelperEngine
    {
        string LoadJson();
		void SaveJson(string data);
    }
    public class HelperEngine:IHelperEngine
	{ 

  
		public HelperEngine()
		{
		}

		public string LoadJson()
		{
            var filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"JsonFile\Status.json");
			using (StreamReader sr = new StreamReader(filePath))
			{
				var json = sr.ReadToEnd();
				return json;
				
			}
		}
		public void SaveJson(string data)
		{
			var filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"JsonFile\Status.json"); 
			

			File.WriteAllText(filePath, data);
		}
	}
}

