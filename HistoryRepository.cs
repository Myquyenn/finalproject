using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ClosedXML.Excel;
using Final_Project;


public class HistoryRepository
{

    private const string ExcelFilePath = "History.xlsx";
    private const string WorksheetName = "History";

    // Ghi danh sách History vào Excel
    public string SaveToExcel(List<History> histories)
    {
        if (histories == null || histories.Count == 0)
        {
            return "Not found";
        }

        XLWorkbook workbook = LoadOrCreateWorkbook();
        IXLWorksheet worksheet = GetOrCreateWorksheet(workbook);

        try
        {
            // Tìm dòng cuối cùng được sử dụng
            int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

            // Nếu file mới, thêm tiêu đề
            if (lastRow == 0)
            {
                worksheet.Cell(1, 1).Value = "Intput";
                worksheet.Cell(1, 2).Value = "Output";
                worksheet.Cell(1, 3).Value = "TimeEvent";
                lastRow = 1;
            }

            // Ghi từng History vào các dòng tiếp theo
            foreach (History history in histories)
            {
                lastRow++;
                worksheet.Cell(lastRow, 1).Value = history.Intput;
                worksheet.Cell(lastRow, 2).Value = history.Output;
                worksheet.Cell(lastRow, 3).Value = history.TimeEvent;
            }

            workbook.SaveAs(ExcelFilePath);
            return "Save success";
        }
        catch (Exception ex)
        {
            throw new IOException($"Lỗi khi lưu danh sách History vào Excel: {ex.Message}", ex);
        }
        finally
        {
            workbook?.Dispose();
        }
    }

    // Ghi một History vào Excel
    public string SaveToExcel(History history)
    {
        if (history == null)
        {
            throw new ArgumentNullException(nameof(history));
        }

        XLWorkbook workbook = LoadOrCreateWorkbook();
        IXLWorksheet worksheet = GetOrCreateWorksheet(workbook);

        try
        {
            // Tìm dòng cuối cùng được sử dụng
            int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

            // Nếu file mới, thêm tiêu đề
            if (lastRow == 0)
            {
                worksheet.Cell(1, 1).Value = "Intput";
                worksheet.Cell(1, 2).Value = "Output";
                worksheet.Cell(1, 3).Value = "TimeEvent";
                lastRow = 1;
            }

            // Ghi History vào dòng tiếp theo
            lastRow++;
            worksheet.Cell(lastRow, 1).Value = history.Intput;
            worksheet.Cell(lastRow, 2).Value = history.Output;
            worksheet.Cell(lastRow, 3).Value = history.TimeEvent;

            workbook.SaveAs(ExcelFilePath);
            return "Save success";
        }
        catch (Exception ex)
        {
            throw new IOException($"Lỗi khi lưu History vào Excel: {ex.Message}", ex);
        }
        finally
        {
            workbook?.Dispose();
        }
    }

    // Đọc danh sách History từ Excel
    public List<History> GetAllHistory()
    {
        if (!File.Exists(ExcelFilePath))
        {
            return new List<History>();
        }

        XLWorkbook workbook = null;
        try
        {
            workbook = new XLWorkbook(ExcelFilePath);
            IXLWorksheet worksheet = workbook.Worksheet(WorksheetName);
            List<History> histories = new List<History>();

            // Bắt đầu từ dòng thứ 2 để bỏ qua tiêu đề
            foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
            {
                // Đọc các giá trị từ cột
                string intput = row.Cell(1).GetString();
                string output = row.Cell(2).GetString();
                DateTime timeEvent;
                bool isValidDate = row.Cell(3).TryGetValue(out timeEvent);

                // Chỉ thêm nếu dữ liệu hợp lệ
                if (!string.IsNullOrEmpty(intput) && isValidDate)
                {
                    histories.Add(new History
                    {
                        Intput = intput,
                        Output = output,
                        TimeEvent = timeEvent
                    });
                }
            }

            // Sắp xếp theo TimeEvent (giảm dần) dựa trên IComparable
            histories.Sort();
            return histories;
        }
        catch (Exception ex)
        {
            throw new IOException($"Lỗi khi đọc History từ Excel: {ex.Message}", ex);
        }
        finally
        {
            workbook?.Dispose();
        }
    }

    //Load hoặc tạo mới workbook
    private XLWorkbook LoadOrCreateWorkbook()
    {
        if (File.Exists(ExcelFilePath))
        {
            return new XLWorkbook(ExcelFilePath);
        }
        return new XLWorkbook();
    }

    //Lấy hoặc tạo worksheet
    private IXLWorksheet GetOrCreateWorksheet(XLWorkbook workbook)
    {
        foreach (var ws in workbook.Worksheets)
        {
            if (ws.Name == WorksheetName)
            {
                return ws;
            }
        }
        return workbook.Worksheets.Add(WorksheetName);
    }

}




