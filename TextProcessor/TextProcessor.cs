using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TextProcessor
{
    public class Text
    {
        private string[] lines;
        public Text(string[] lines) { this.lines = lines; }
        public string this[int i] { get { return lines[i]; } }
        public int Length { get { return lines.Length; } }
    }

    [ContentImporter(".txt", DefaultProcessor = "TextProcessor",
      DisplayName = "Text File Importer")]
    public class TextImporter : ContentImporter<Text>
    {
        public override Text Import(string filename, ContentImporterContext context)
        {
            List<string> imported = new List<string>();
            using (TextReader tr = new StreamReader(filename))
            {
                string str = tr.ReadLine();
                while (str != null)
                {
                    if (str.Length > 0)
                        imported.Add(str);
                    str = tr.ReadLine();
                }
            }
            return new Text(imported.ToArray());
        }
    }

    /*public class CompiledText
    {
        private string str;
        public CompiledText(string str)
        {
            this.str = str;
        }
        public string Data { get { return str; } }
    }*/

    [ContentProcessor(DisplayName = "Text File Processor")]
    public class TextProcessor : ContentProcessor<Text, Text>
    {
        public override Text Process(Text input, ContentProcessorContext context)
        {
            return input;
            /*if (input.Length == 0)
                return new CompiledText("");
            string s = "";
            for (int i = 0; i < input.Length - 1; i++)
                s += input[i] + '\n';
            s += input[input.Length - 1];
            return new CompiledText(s);*/
        }
    }

    [ContentTypeWriter]
    public class TextWriter : ContentTypeWriter<Text>
    {
        protected override void Write(ContentWriter output, Text value)
        {
            //output.Write(value.Length);
            for (int i = 0; i < value.Length; i++)
                output.Write(value[i]);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "RibbonsRedux.Content.TextReader, RibbonsRedux, Version=1.0.0.0, Culture=neutral";
        }
    }
}