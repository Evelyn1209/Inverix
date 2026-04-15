using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;

// Librerías para Excel (ClosedXML)
using ClosedXML.Excel;

// Librerías para PDF (iText)
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.IO.Image;

namespace Inverix.Classes
{
    public class ReporteClass
    {
        /// <summary>
        /// Genera un archivo Excel real (.xlsx) y lo descarga en el navegador.
        /// </summary>
        public static void GenerarExcel(DataTable dt, string titulo, HttpResponse response)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte");

                // Título del Reporte
                worksheet.Cell(1, 1).Value = titulo;
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                worksheet.Range(1, 1, 1, dt.Columns.Count).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Insertar los datos de la tabla (empieza en la fila 3 para dejar espacio al título)
                var tabla = worksheet.Cell(3, 1).InsertTable(dt);
                tabla.Theme = XLTableTheme.TableStyleMedium9; // Un estilo azul profesional

                // --- AÑADIR FILA DE TOTALES ---
                int lastRow = dt.Rows.Count + 3;
                int rowTotales = lastRow + 1;

                worksheet.Cell(rowTotales, 1).Value = "TOTALES ACUMULADOS";
                worksheet.Cell(rowTotales, 1).Style.Font.Bold = true;

                // Sumatorias (Asumiendo que las columnas numéricas están en posiciones específicas tras el cambio en el SQL)
                // Inversión (Col 5), InteresBruto (Col 6), Impuesto (Col 7), GananciaNeta (Col 8)
                decimal totalInv = 0, totalBruto = 0, totalImp = 0, totalNeto = 0;
                foreach (DataRow row in dt.Rows)
                {
                    totalInv += Convert.ToDecimal(row["Inversion"]);
                    totalBruto += Convert.ToDecimal(row["InteresBruto"]);
                    totalImp += Convert.ToDecimal(row["Impuesto"]);
                    totalNeto += Convert.ToDecimal(row["GananciaNeta"]);
                }

                worksheet.Cell(rowTotales, 5).Value = totalInv;
                worksheet.Cell(rowTotales, 6).Value = totalBruto;
                worksheet.Cell(rowTotales, 7).Value = totalImp;
                worksheet.Cell(rowTotales, 8).Value = totalNeto;

                // Estilo para la fila de totales
                var rangoTotales = worksheet.Range(rowTotales, 1, rowTotales, dt.Columns.Count);
                rangoTotales.Style.Font.Bold = true;
                rangoTotales.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4F8");

                // Ajustar columnas
                worksheet.Columns().AdjustToContents();

                // Configurar Response para descarga
                response.Clear();
                response.Buffer = true;
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.AddHeader("content-disposition", "attachment;filename=" + titulo.Replace(" ", "_") + ".xlsx");

                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    ms.WriteTo(response.OutputStream);
                    response.Flush();
                    response.End();
                }
            }
        }

        /// <summary>
        /// Genera un documento PDF formal y lo descarga en el navegador.
        /// </summary>
        public static void GenerarPDF(DataTable dt, string titulo, string nombreCotizante, HttpResponse response)
        {
            response.Clear();
            response.ContentType = "application/pdf";
            response.AddHeader("content-disposition", "attachment;filename=" + titulo.Replace(" ", "_") + ".pdf");

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                pdf.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A4.Rotate()); // Rotar a Horizontal
                Document document = new Document(pdf);

                // --- ENCABEZADO ---
                string logoPath = HttpContext.Current.Server.MapPath("~/Images/iconlogo.png");
                if (File.Exists(logoPath))
                {
                    ImageData data = ImageDataFactory.Create(logoPath);
                    Image img = new Image(data);
                    img.SetWidth(50);
                    img.SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    document.Add(img);
                }

                Paragraph header = new Paragraph(titulo);
                header.SetTextAlignment(TextAlignment.CENTER);
                header.SetFontSize(20);
                // header.SetBold(); // Comentado temporalmente para evitar error
                header.SetFontColor(new DeviceRgb(22, 147, 165));
                document.Add(header);

                Paragraph subtitulo = new Paragraph("Generado para: " + nombreCotizante);
                subtitulo.SetTextAlignment(TextAlignment.LEFT);
                subtitulo.SetFontSize(10);
                // subtitulo.SetItalic(); // Comentado temporalmente para evitar error
                document.Add(subtitulo);

                Paragraph fechaEmision = new Paragraph("Fecha de emisión: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                fechaEmision.SetTextAlignment(TextAlignment.LEFT);
                fechaEmision.SetFontSize(10);
                fechaEmision.SetMarginBottom(10);
                document.Add(fechaEmision);

                // --- CÁLCULO DE TOTALES PARA RESUMEN ---
                decimal sumInv = 0, sumNeto = 0;
                foreach (DataRow r in dt.Rows)
                {
                    sumInv += Convert.ToDecimal(r["Inversion"]);
                    sumNeto += Convert.ToDecimal(r["GananciaNeta"]);
                }

                // --- CUADROS DE RESUMEN EJECUTIVO ---
                Table summaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }));
                summaryTable.SetWidth(UnitValue.CreatePercentValue(100));
                summaryTable.SetMarginBottom(20);

                // Cuadro Inversión
                Cell cellInv = new Cell();
                cellInv.SetBackgroundColor(new DeviceRgb(235, 245, 255));
                cellInv.SetPadding(10);
                cellInv.Add(new Paragraph("INVERSIÓN TOTAL").SetFontSize(9).SetFontColor(ColorConstants.GRAY));
                Paragraph valInv = new Paragraph(string.Format("${0:N2}", sumInv)).SetFontSize(16).SetFontColor(new DeviceRgb(22, 147, 165));
                cellInv.Add(valInv);
                summaryTable.AddCell(cellInv);

                // Cuadro Ganancia
                Cell cellGain = new Cell();
                cellGain.SetBackgroundColor(new DeviceRgb(235, 255, 245));
                cellGain.SetPadding(10);
                cellGain.Add(new Paragraph("GANANCIA NETA TOTAL").SetFontSize(9).SetFontColor(ColorConstants.GRAY));
                Paragraph valGain = new Paragraph(string.Format("${0:N2}", sumNeto)).SetFontSize(16).SetFontColor(new DeviceRgb(40, 167, 69));
                cellGain.Add(valGain);
                summaryTable.AddCell(cellGain);

                document.Add(summaryTable);

                // --- TABLA DE DATOS ---
                // Omitimos IdCotizacion (columna 0) para el reporte
                int columnCount = dt.Columns.Count - 1; 
                Table table = new Table(UnitValue.CreatePercentArray(columnCount));
                table.SetWidth(UnitValue.CreatePercentValue(100));

                // Cabeceras
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    DataColumn column = dt.Columns[i];
                    Paragraph pHeader = new Paragraph(column.ColumnName);
                    pHeader.SetFontColor(ColorConstants.WHITE);
                    pHeader.SetFontSize(9);

                    Cell cell = new Cell();
                    cell.Add(pHeader);
                    cell.SetBackgroundColor(new DeviceRgb(22, 147, 165));
                    cell.SetTextAlignment(TextAlignment.CENTER);
                    table.AddHeaderCell(cell);
                }

                // Celdas
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        var cellValue = row[i];
                        string val = cellValue.ToString();
                        if (cellValue is DateTime) val = ((DateTime)cellValue).ToString("dd/MM/yyyy");
                        if (cellValue is decimal || cellValue is double) val = string.Format("{0:N2}", cellValue);

                        Paragraph pCell = new Paragraph(val);
                        pCell.SetFontSize(8);

                        Cell cell = new Cell();
                        cell.Add(pCell);
                        cell.SetTextAlignment(TextAlignment.CENTER);
                        table.AddCell(cell);
                    }
                }

                document.Add(table);

                // --- PIE DE PÁGINA ---
                Paragraph footer = new Paragraph("\n\nSistema Financiero Inverix - Reporte Oficial");
                footer.SetTextAlignment(TextAlignment.CENTER);
                footer.SetFontSize(8);
                footer.SetFontColor(ColorConstants.GRAY);
                document.Add(footer);

                document.Close();

                byte[] bytes = ms.ToArray();
                response.OutputStream.Write(bytes, 0, bytes.Length);
                response.Flush();
                response.End();
            }
        }
    }
}