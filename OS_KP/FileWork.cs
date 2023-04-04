using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace OS_KP
{
    class FileWork
    {
        public static void OpenFile(string path, out string pages, out int memoryCount)
        {
            string str = null;
            str = System.IO.File.ReadAllText(path);


            string[] stringSeparators = { "\r", "\n", " ", "\t" };
            List<int> tempList = str.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(n => int.Parse(n))
                                    .ToList();
            memoryCount = tempList[0];
            tempList.RemoveAt(0);
            pages = string.Join(" ", tempList);
            path = string.Empty;
        }

        public static void CreatePDF(string path, byte[] bitmap, string nruLog, string lruLog, string fifoLog, string scLog, string clockLog )
        {
            string lucidaConsolePath = "../../resources/lucidaConsole.ttf";
            BaseFont bf1 = BaseFont.CreateFont(lucidaConsolePath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font lucidaConsole = new Font(bf1, 10, Font.NORMAL);

            string timesNewRomanPath = "../../resources/timesNewRoman.ttf";
            BaseFont bf2 = BaseFont.CreateFont(timesNewRomanPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font timesNewRoman = new Font(bf2, 20, Font.BOLD);

            Document doc = new Document(PageSize.A4);
            var writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create)); 
            doc.Open();

            var header = new Paragraph("Анализ методов замещения страниц\n", timesNewRoman);
            header.Alignment = Element.ALIGN_CENTER;
            doc.Add(header);

            var image = Image.GetInstance(bitmap);
            var pageWidth = doc.PageSize.Width - 100;
            var pageHeight = doc.PageSize.Height - 400;
            image.ScaleToFit(pageWidth, pageHeight);
            image.Alignment = Element.ALIGN_CENTER;
            doc.Add(image);

            var nruLogParagraph = new Paragraph(nruLog + "\n\n", lucidaConsole);
            nruLogParagraph.Alignment = Element.ALIGN_JUSTIFIED;
            doc.Add(nruLogParagraph);

            var lruLogParagraph = new Paragraph(lruLog + "\n\n", lucidaConsole);
            lruLogParagraph.Alignment = Element.ALIGN_JUSTIFIED;
            doc.Add(lruLogParagraph);

            var fifoLogParagraph = new Paragraph(fifoLog + "\n\n", lucidaConsole);
            fifoLogParagraph.Alignment = Element.ALIGN_JUSTIFIED;
            doc.Add(fifoLogParagraph);

            var scLogParagraph = new Paragraph(scLog + "\n\n", lucidaConsole);
            scLogParagraph.Alignment = Element.ALIGN_JUSTIFIED;
            doc.Add(scLogParagraph);

            var clockLogParagraph = new Paragraph(clockLog + "\n\n", lucidaConsole);
            clockLogParagraph.Alignment = Element.ALIGN_JUSTIFIED;
            doc.Add(clockLogParagraph);

            doc.Close();
        }
    }
}
