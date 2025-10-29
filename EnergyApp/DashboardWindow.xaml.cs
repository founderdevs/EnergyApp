using EnergyApp.Data;
using EnergyApp.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;

namespace EnergyApp
{
    public partial class DashboardWindow : Window
    {
        private List<EnergyRecord> allRecords = new();

        public DashboardWindow()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            LoadData();
        }

        private void LoadData()
        {
            allRecords = DatabaseHelper.GetRecords();
            EnergyDataGrid.ItemsSource = allRecords;
            UpdateSummary(allRecords);
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите обе даты для фильтрации.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var start = StartDatePicker.SelectedDate.Value;
            var end = EndDatePicker.SelectedDate.Value;

            if (start > end)
            {
                MessageBox.Show("Начальная дата не может быть позже конечной.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var filtered = allRecords.Where(r => r.Date >= start && r.Date <= end).ToList();
            EnergyDataGrid.ItemsSource = filtered;
            UpdateSummary(filtered);
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            EnergyDataGrid.ItemsSource = allRecords;
            UpdateSummary(allRecords);
        }

        private void UpdateSummary(List<EnergyRecord> records)
        {
            double totalConsumption = records.Sum(r => r.Consumption);
            double totalCost = records.Sum(r => r.Cost);
            SummaryText.Text = $"Итого: {totalConsumption} кВт⋅ч | {totalCost:F2} ₽";
        }

        private void AddRecordButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddRecordWindow();
            if (addWindow.ShowDialog() == true)
            {
                DatabaseHelper.AddRecord(addWindow.NewRecord);
                LoadData();
            }
        }

        private void DeleteRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnergyDataGrid.SelectedItem is EnergyRecord selected)
            {
                DatabaseHelper.DeleteRecord(selected.Id);
                LoadData();
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления");
            }
        }

        private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Энергия",
                DefaultExt = ".pdf",
                Filter = "PDF documents (.pdf)|*.pdf"
            };

            if (dialog.ShowDialog() != true) return;

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Учёт электроэнергии";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont headerFont = new XFont("Times New Roman", 16, XFontStyle.Bold);
            XFont tableFont = new XFont("Times New Roman", 12, XFontStyle.Regular);

            double yPoint = 40;

            // Заголовок
            gfx.DrawString("Учёт электроэнергии", headerFont, XBrushes.Black,
                new XRect(0, yPoint, page.Width, 20),
                XStringFormats.TopCenter);

            yPoint += 40;

            // Таблица
            double x = 40;
            double rowHeight = 20;

            // Заголовки колонок
            string[] headers = { "Дата", "Расход (кВт⋅ч)", "Цена (₽/кВт⋅ч)", "Стоимость (₽)" };
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawString(headers[i], tableFont, XBrushes.Black, new XRect(x + i * 120, yPoint, 120, rowHeight), XStringFormats.TopLeft);
            }
            yPoint += rowHeight;

            // Данные
            foreach (EnergyRecord rec in allRecords)
            {
                gfx.DrawString(rec.Date.ToString("yyyy-MM-dd"), tableFont, XBrushes.Black, new XRect(x + 0 * 120, yPoint, 120, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(rec.Consumption.ToString(), tableFont, XBrushes.Black, new XRect(x + 1 * 120, yPoint, 120, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(rec.PricePerKwh.ToString("F2"), tableFont, XBrushes.Black, new XRect(x + 2 * 120, yPoint, 120, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(rec.Cost.ToString("F2"), tableFont, XBrushes.Black, new XRect(x + 3 * 120, yPoint, 120, rowHeight), XStringFormats.TopLeft);
                yPoint += rowHeight;
            }

            document.Save(dialog.FileName);
            Process.Start(new ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
        }
    }
}