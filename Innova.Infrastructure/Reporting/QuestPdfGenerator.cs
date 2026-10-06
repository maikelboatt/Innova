// using Innova.Application.Abstractions.Services;
// using QuestPDF.Fluent;
// using QuestPDF.Helpers;
//
// namespace Innova.Infrastructure.Reporting
// {
//     public sealed class QuestPdfGenerator:IPdfGenerator
//     {
//         public byte[] GenerateBillReceipt( BillReceiptData data )
//         {
//             Document doc = Document.Create(container =>
//             {
//                 container.Page(page =>
//                 {
//                     page.Size(PageSizes.A5);
//                     page.Margin(30);
//                     page
//                         .Header()
//                         .Text("MediCore Clinic — Receipt")
//                         .Bold()
//                         .FontSize(16);
//
//                     page
//                         .Content()
//                         .Column(col =>
//                         {
//                             col
//                                 .Item()
//                                 .Text($"Patient: {data.PatientName}");
//                             col
//                                 .Item()
//                                 .Text($"Bill ID: {data.BillId}");
//                             col
//                                 .Item()
//                                 .Text($"Date: {data.IssuedAt:dd MMM yyyy}");
//                             col
//                                 .Item()
//                                 .PaddingVertical(10)
//                                 .LineHorizontal(1);
//
//                             col
//                                 .Item()
//                                 .Table(table =>
//                                 {
//                                     table.ColumnsDefinition(c =>
//                                     {
//                                         c.RelativeColumn(3);
//                                         c.RelativeColumn();
//                                         c.RelativeColumn();
//                                     });
//                                     table.Header(h =>
//                                     {
//                                         h
//                                             .Cell()
//                                             .Text("Description")
//                                             .Bold();
//                                         h
//                                             .Cell()
//                                             .Text("Qty")
//                                             .Bold();
//                                         h
//                                             .Cell()
//                                             .Text("Amount")
//                                             .Bold();
//                                     });
//                                     foreach (LineItemData li in data.LineItems)
//                                     {
//                                         table
//                                             .Cell()
//                                             .Text(li.Description);
//                                         table
//                                             .Cell()
//                                             .Text(li.Quantity.ToString());
//                                         table
//                                             .Cell()
//                                             .Text((li.UnitPrice * li.Quantity).ToString("C"));
//                                     }
//                                 });
//
//                             col
//                                 .Item()
//                                 .PaddingTop(10)
//                                 .AlignRight()
//                                 .Text($"Total: {data.Total:C}")
//                                 .Bold();
//                             col
//                                 .Item()
//                                 .AlignRight()
//                                 .Text($"Paid: {data.Paid:C}");
//                         });
//
//                     page
//                         .Footer()
//                         .AlignCenter()
//                         .Text("Thank you for visiting MediCore Clinic.");
//                 });
//             });
//
//             return doc.GeneratePdf();
//         }
//
//         public byte[] GeneratePrescriptionSlip( PrescriptionSlipData data )
//         {
//             Document doc = Document.Create(container =>
//             {
//                 container.Page(page =>
//                 {
//                     page.Size(PageSizes.A5);
//                     page.Margin(30);
//                     page
//                         .Header()
//                         .Text("MediCore Clinic — Prescription")
//                         .Bold()
//                         .FontSize(16);
//
//                     page
//                         .Content()
//                         .Column(col =>
//                         {
//                             col
//                                 .Item()
//                                 .Text($"Patient: {data.PatientName}");
//                             col
//                                 .Item()
//                                 .Text($"Prescribing Doctor: {data.DoctorName}");
//                             col
//                                 .Item()
//                                 .Text($"Date: {data.IssuedAt:dd MMM yyyy}");
//                             col
//                                 .Item()
//                                 .PaddingVertical(10)
//                                 .LineHorizontal(1);
//
//                             foreach (MedicationLineData m in data.Medications)
//                             {
//                                 col
//                                     .Item()
//                                     .PaddingBottom(6)
//                                     .Text(text =>
//                                     {
//                                         text
//                                             .Line($"{m.MedicationName} — {m.DosageAmount} {m.DosageUnit}")
//                                             .Bold();
//                                         text.Line($"{m.FrequencyTimesPerDay}x daily for {m.DurationValue} {m.DurationUnit}");
//                                     });
//                             }
//                         });
//                 });
//             });
//
//             return doc.GeneratePdf();
//         }
//     }
// }


