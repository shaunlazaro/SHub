using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using Newtonsoft.Json;
using Tutorial.Controls;

namespace Tutorial.Managers
{
    public class UserInfoManager
    {
        #region Boilerplate
        private UserInfoManager() { }
        static Lazy<UserInfoManager> instance = new Lazy<UserInfoManager>(() => new UserInfoManager());
        public static UserInfoManager Instance { get => instance.Value; }

        UserData userData;
        string userFilePath = "";

        public async Task Init()
        {
            string directory = await Electron.App.GetPathAsync(ElectronNET.API.Entities.PathName.UserData);
            userFilePath = $@"{directory}\preferences.json";
            Console.WriteLine($"Init: Save path set to {userFilePath}");
            await Load();
        }
        #endregion

        // Access using UserInfoManager.Instance.Variable
        // Public variables that are part of UserData should be accessed through mUserData in a getter,
        // and the setter should save if modifying directly is allowed.
        #region Public Variables
        public int NumberOfSavesPerformed { get => userData.numberOfSavesPerformed; set { userData.numberOfSavesPerformed = value; } }
        public bool MacrosListening { get => userData.macrosListening; set { userData.macrosListening = value; Save(); } }
        public List<MacroDetails> CustomMacros { get => userData.customMacros; 
            set 
            {
                List<MacroDetails> blackList = Helpers.DefaultMacroDetails();
                List<MacroDetails> rawList = value;
                List<MacroDetails> newList = new List<MacroDetails>();

                foreach(MacroDetails details in rawList)
                {
                    bool elementBlackListed = false;
                    foreach(MacroDetails banned in blackList)
                    {
                        if (details.name == banned.name)
                        {
                            elementBlackListed = true;
                            break;
                        }
                    }

                    if (!elementBlackListed)
                        newList.Add(details);
                }
                
                foreach (MacroDetails details in newList)
                    Console.WriteLine("Macro to be saved: " + details.name);
                userData.customMacros = newList;
                Save(); 
            } 
        } // Make sure default macros are never saved here.
        #endregion

        #region I/O
        private async Task Load()
        {
            Console.WriteLine("Looking for user data: " + userFilePath);

            if (!File.Exists(userFilePath))
            {
                userData = new UserData(); // no file path found, start with default
                return;
            }

            string text;
            using (StreamReader streamReader = new StreamReader(userFilePath))
            {
                text = await streamReader.ReadToEndAsync();
            }

            userData = JsonConvert.DeserializeObject<UserData>(text);
            if (userData == null) userData = new UserData();
        }

        public void Save()
        {
            userData.numberOfSavesPerformed++;
            Console.WriteLine($"Saving... {userData.numberOfSavesPerformed} saves performed.");

            string textToWrite = JsonConvert.SerializeObject(userData);
            using (StreamWriter streamWriter = new StreamWriter(userFilePath))
            {
                streamWriter.Write(textToWrite);
            }
        }
        #endregion
    }

    [Serializable]
    class UserData  // It's a model in a manager folder :)
    {
        public int numberOfSavesPerformed = 0;
        public bool macrosListening = false;
        public List<MacroDetails> customMacros = new List<MacroDetails>();
    }
}
