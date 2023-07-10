
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;
namespace Pdf.Helper;
public static class PdfConvert {
public static  void ConvertDocxToPdf(string docxFilePath, string pdfFilePath)
{
    // Create a new Microsoft Word application instance
    var wordApp = new Application();

    try
    {
        // Open the source document
        var doc = wordApp.Documents.Open(docxFilePath);

        // Save the document as PDF
        doc.SaveAs2(pdfFilePath, WdSaveFormat.wdFormatPDF);

        // Close the document and the Word application
        doc.Close();
        wordApp.Quit();
        ReleaseComObject(doc);
        ReleaseComObject(wordApp);
    }
    catch (Exception ex)
    {
        // Handle any exceptions that occur
        Console.WriteLine("Error: " + ex.Message);
    }
    finally
    {
        // Release the COM objects
        // ReleaseComObject(doc);
        // ReleaseComObject(wordApp);
    }
}

private static  void ReleaseComObject(object obj)
{
    try
    {
        if (obj != null)
        {
            Marshal.ReleaseComObject(obj);
            obj = null;
        }
    }
    catch (Exception ex)
    {
        obj = null;
        Console.WriteLine("Error releasing COM object: " + ex.Message);
    }
    finally
    {
        GC.Collect();
    }
}

}