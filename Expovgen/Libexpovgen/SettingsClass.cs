using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Expovgen;
using Expovgen.ImgFetch;
using Expovgen.LangAPI;

namespace Expovgen
{
    /// <summary>
    /// Holds definitions for Expovgen video/podcast generation
    /// </summary>
    public class ExpovgenGenerationSettings
    {
        // Visual generation window definitions
        public WindowStyle WindowStyle { get; set; } = WindowStyle.Simple;

        // Background work definitions
        public string PythonPath { get; set; } = "";

        // Definitions for step customization
        public Etapa1Behaviors Etapa1Behaviors { get; set; } = Etapa1Behaviors.Auto;
        public Etapa2Behaviors Etapa2Behaviors { get; set; } = Etapa2Behaviors.Auto;
        public Etapa3Behaviors Etapa3Behaviors { get; set; } = Etapa3Behaviors.Auto;

        //Content generation type
        public GenerationType GenerationType { get; set; } = GenerationType.VideoGen;

        // Definitions for rendering/saving videos/project files
        public int VideoWidth { get; set; } = 1366;
        public int VideoHeight { get; set; } = 768;
        public string? ExportPath { get; set; }
        public string[] Document { get; set; }
        
        // Other Service options
        public Services ImgFetchService { get; set; } = Services.pixabay;
        public bool UseCCLicense { get; set; } = false;

        // Definitions for video rendering customization
        public string? BackgroundMusicPath { get; set; }
        public double FontSize { get; set; } = 20; 
        public string[]? TitleCard { get; set; }
        public string[]? Credits { get; set; }
        public double BackgroundVolume { get; set; } = 0.2;
        // TODO: Update stitchtest2 & imgfetch in order to make these possible
        public bool ShowKeywordOnImage { get; set; } = false;


        /// <summary>
        /// Check if all settings are valid for video generation
        /// </summary>
        /// <returns>True if settings are valid, otherwise false</returns>
        public List<string>? CheckValid()
        {
            List<string>? msgs = new();

            //BEGIN: Conditions for validity ----------------

            if (ExportPath is null)
                msgs.Add("Favor adicione um caminho de destino do vídeo");

            //END: Conditions for validity ------------------

            if (msgs.Count == 0)
                msgs = null;
            return msgs;
        }

        /// <summary>
        /// Serializes current <see cref="ExpovgenGenerationSettings"/> object onto JSON format and saves it to a file
        /// </summary>
        /// <returns>True if saving was successful, otherwise false</returns>
        public bool Save(string destFileName)
        {
            string content = JsonSerializer.Serialize(this);
            try
            {
                File.WriteAllText(destFileName, content);
                return true;
            } catch { return false; }
        }

        /// <summary>
        /// Reads a file and attempts to extract an <see cref="ExpovgenGenerationSettings"/> object from it
        /// </summary>
        public static ExpovgenGenerationSettings? FromFile(string fileName)
        {
            try
            {
                string serializedContent = File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<ExpovgenGenerationSettings>(serializedContent);
            }
            catch { return null; }
        }
    }

}
