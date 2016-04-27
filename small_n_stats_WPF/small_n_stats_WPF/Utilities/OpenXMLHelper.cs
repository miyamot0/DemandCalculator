/* 
    Copyright 2016 Shawn Gilroy

    This file is part of Small N Stats.

    Small N Stats is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 2.

    Small N Stats is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Small N Stats.  If not, see <http://www.gnu.org/licenses/gpl-2.0.html>.

*/

using small_n_stats_WPF.ViewModels;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.ObjectModel;

namespace small_n_stats_WPF.Utilities
{
    public class OpenXMLHelper
    {
        /// <summary>
        /// Write contents of RowModels to spreadsheet
        /// <param name="rowCollection">
        /// Contents of data grid
        /// </param>
        /// <param name="filePath">
        /// Output location for .xlsx file
        /// </param>
        /// </summary>
        public static void ExportToExcel(ObservableCollection<RowViewModel> rowCollection, string filePath)
        {
            using (var workbook = SpreadsheetDocument.Create(filePath, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var wbPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() {
                    Id = relationshipId,
                    SheetId = 1,
                    Name = "Small N Stats"
                };

                sheets.Append(sheet);

                foreach (RowViewModel row in rowCollection)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    foreach (String col in row.values)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(col);
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }
            }
        }
    }
}