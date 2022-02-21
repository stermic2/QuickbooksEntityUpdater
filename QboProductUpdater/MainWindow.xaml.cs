using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic.FileIO;
using QboProductUpdater.Extensions;
using QuickBooksSharp;
using QuickBooksSharp.Entities;

namespace QboProductUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Quickbooks.InitializeQuickbooksClient();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Quickbooks.RefreshTokens();
            using (TextFieldParser parser = new TextFieldParser(@"C:\Users\mikie.TLOFFICE\Desktop\NataliasChanges2.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields();
                    var item = (await Quickbooks.DataService.QueryAsync<Item>("SELECT * FROM Item WHERE Sku = " + fields[1].AddSingleParenthesis())).Response?.Entities?.FirstOrDefault();
                    if (item != null)
                    {
                        try
                        {
                            var result = await Quickbooks.DataService.PostAsync(new Item
                            {
                                Id = item.Id,
                                SyncToken = item.SyncToken,
                                Name = fields[0],
                                sparse = true
                            });
                        }
                        catch (QuickBooksException ex)
                        {
                            if (!ex.Message.Contains("Duplicate Name Exists Error")) 
                                throw;
                            Console.WriteLine("Sku error:" + fields[1]);
                        }
                    }
                }
            }
        }

        private static async Task ChangeAccountReferenceStartingFrom20210301ToPresent()
        {
            await Quickbooks.RefreshTokens();
            var salesReceipts =
                await Quickbooks.DataService.QueryAsync<SalesReceipt>(
                    "SELECT * FROM SalesReceipt WHERE TxnDate >= '2021-03-01' AND  MAXRESULTS 30");
            for (int i = 1; salesReceipts.Response != null && salesReceipts.Response.Entities!.Any(); i++)
            {
                List<IntuitResponse<BatchItemResponse[]>> responses = new List<IntuitResponse<BatchItemResponse[]>>();
                foreach (var batch in salesReceipts.Response.Entities.BatchesOf(30))
                {
                    responses.Add(await Quickbooks.DataService.BatchAsync(new IntuitBatchRequest
                    {
                        BatchItemRequest = batch.Select(salesReceipt => new BatchItemRequest
                        {
                            bId = Guid.NewGuid().ToString(),
                            operation = OperationEnum.update,
                            SalesReceipt = salesReceipt.changeAccountReference()
                        }).ToArray()
                    }));
                }

                salesReceipts = await Quickbooks.DataService.QueryAsync<SalesReceipt>(
                    "SELECT * FROM SalesReceipt WHERE TxnDate >= '2021-03-01' MAXRESULTS 30 STARTPOSITION " + i * 30);
            }
        }

        private static async Task SetProductDescriptionAndNameToTheBeTheSame()
        {
            await Quickbooks.RefreshTokens();
            var items = await Quickbooks.DataService.QueryAsync<Item>("SELECT * FROM Item MAXRESULTS 30");
            for (int i = 1; items.Response != null && items.Response.Entities!.Any(); i++)
            {
                List<IntuitResponse<BatchItemResponse[]>> responses = new List<IntuitResponse<BatchItemResponse[]>>();
                foreach (var batch in items.Response.Entities.BatchesOf(30))
                {
                    responses.Add(await Quickbooks.DataService.BatchAsync(new IntuitBatchRequest
                    {
                        BatchItemRequest = batch.Select(item => new BatchItemRequest
                        {
                            bId = Guid.NewGuid().ToString(),
                            operation = OperationEnum.update,
                            Item = item.makeDescriptionSameAsName()
                        }).ToArray()
                    }));
                }

                items = await Quickbooks.DataService.QueryAsync<Item>(
                    "SELECT * FROM Item MAXRESULTS 30 STARTPOSITION " + i * 30);
            }
        }
    }
}