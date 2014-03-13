using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.ApplicationModel;

using FyreVM;

namespace fyrexaml.Services
{
    public class GameService
    {
        private Engine vm = null;
        private string command = "";
        private Stream restoreData = null;

        public Dictionary<string, string> ChannelData { get; private set; }

        public GameService()
        {

        }

        public async Task LoadGame() {

            StorageFile gameFile = await Package.Current.InstalledLocation.GetFileAsync(@"Game\shadow-w8.ulx");
            byte[] gameFileData = await ReadFromFile(gameFile);
            Stream gameStream = new MemoryStream(gameFileData);

            vm = new Engine(gameStream);

            Run();
        }

        public void Run()
        {
            vm.OutputReady += new OutputReadyEventHandler(vm_OutputReady);
            vm.LineWanted += new LineWantedEventHandler(vm_LineWanted);
            vm.KeyWanted += new KeyWantedEventHandler(vm_KeyWanted);
            vm.SaveRequested += new SaveRestoreEventHandler(vm_SaveRequested);
            vm.LoadRequested += new SaveRestoreEventHandler(vm_LoadRequested);

            vm.Run();
        }

        public void SendCommand(string userCommand)
        {
            command = userCommand;

            if (vm == null)
            {
                throw new Exception("Engine is not running.");
            }
        }

        private void vm_OutputReady(object sender, OutputReadyEventArgs e)
        {
            if (e.Package.Keys.Count > 0)
            {
                if (command.ToLower() == "save" || command.ToLower() == "restore")
                {
                    return;
                }

                ChannelData = (Dictionary<string, string>)e.Package;
            }
        }

        private void vm_LineWanted(object sender, LineWantedEventArgs e)
        {
            e.Line = command;
        }

        private void vm_KeyWanted(object sender, KeyWantedEventArgs e)
        {
            e.Char = command[0];
        }

        private void vm_SaveRequested(object sender, SaveRestoreEventArgs e)
        {
            e.Stream = new MemoryStream();
        }

        private void vm_LoadRequested(object sender, SaveRestoreEventArgs e)
        {
            e.Stream = restoreData;
        }

        public async Task<byte[]> ReadFromFile(StorageFile gameFile)
        {
            byte[] gameFileArray = null;

            using (IRandomAccessStream sessionRandomAccess =
                await gameFile.OpenAsync(FileAccessMode.Read))
            {
                if (sessionRandomAccess.Size > 0)
                {
                    gameFileArray = new byte[sessionRandomAccess.Size];
                    IBuffer output = await
                                sessionRandomAccess.ReadAsync(
                                    gameFileArray.AsBuffer(0, (int)sessionRandomAccess.Size),
                                    (uint)sessionRandomAccess.Size,
                                    InputStreamOptions.Partial);
                }
            }

            return gameFileArray;
        }

    }
}
