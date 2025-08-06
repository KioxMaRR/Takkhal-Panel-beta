using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Windows.Forms;
using System.Drawing;

namespace Takkhal_Panel
{
    public static class Logger
    {
        private static RichTextBox _outputBox;

        public static void Initialize(RichTextBox outputBox)
        {
            _outputBox = outputBox;
        }

        public static void Log(string message)
        {
            if (_outputBox == null)
                return;

            string timestamp = DateTime.Now.ToString("HH:mm:ss");

            _outputBox.Invoke((MethodInvoker)(() =>
            {
                _outputBox.SelectionStart = _outputBox.TextLength;
                _outputBox.SelectionLength = 0;

          
                _outputBox.SelectionColor = Color.Red;
                _outputBox.AppendText($"[{timestamp}] [TK] ");


                _outputBox.SelectionColor = Color.White;
                _outputBox.AppendText(message + Environment.NewLine);

                _outputBox.SelectionColor = _outputBox.ForeColor;
                _outputBox.ScrollToCaret();
            }));
        }
        public static void UpdateLastLine(string message)
        {
            if (_outputBox == null)
                return;

            _outputBox.Invoke((MethodInvoker)(() =>
            {
                string[] lines = _outputBox.Lines;

                if (lines.Length == 0)
                {
              
                    Log(message);
                    return;
                }

                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                string newLine = $"[{timestamp}] [TK] {message}";

                lines[lines.Length - 1] = newLine;
                _outputBox.Lines = lines;

                _outputBox.SelectionStart = _outputBox.Text.Length;
                _outputBox.ScrollToCaret();
            }));
        }


    }
}