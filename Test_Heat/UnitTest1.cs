using NUnit.Framework;
using Excel = Microsoft.Office.Interop.Excel;
using Heat_Lib;
using System.IO;
using System;


namespace Test_Heat
{
    public class Tests
    {
        private string fileName = "ForTest.xlsx";
        Excel.Application objExcel = null;
        Excel.Workbook WorkBook = null;

        private object oMissing = System.Reflection.Missing.Value;

        [Test]
        public void CalculationTest()
        {
            CalculationClass Calc = new CalculationClass();

            Calc.h_layer = 6.00;
            Calc.t_beg_mat = 650.0;
            Calc.t_beg_gaz = 10.0;
            Calc.v_gaz = 0.60;
            Calc.c_gaz = 1.34;
            Calc.mat_cons = 1.70;
            Calc.c_mat = 1.49;
            Calc.lamda_v = 2450.0;
            Calc.d_mash = 2.10;

            try
            {

                objExcel = new Excel.Application();
                WorkBook = objExcel.Workbooks.Open(
                            Directory.GetCurrentDirectory() + "\\" + fileName);
                Excel.Worksheet WorkSheet = (Excel.Worksheet)WorkBook.Sheets["Лист1"];

                ((Microsoft.Office.Interop.Excel.Range)WorkSheet.Cells[2, 7]).Value2 = Calc.h_layer;
                ((Microsoft.Office.Interop.Excel.Range)WorkSheet.Cells[3, 7]).Value2 = Calc.t_beg_mat;
                ((Microsoft.Office.Interop.Excel.Range)WorkSheet.Cells[4, 7]).Value2 = Calc.t_beg_gaz;
                ((Microsoft.Office.Interop.Excel.Range)WorkSheet.Cells[5, 7]).Value2 = Calc.v_gaz;
                ((Microsoft.Office.Interop.Excel.Range)WorkSheet.Cells[6, 7]).Value2 = Calc.c_gaz;
                ((Microsoft.Office.Interop.Excel.Range)WorkSheet.Cells[7, 7]).Value2 = Calc.mat_cons;
                ((Microsoft.Office.Interop.Excel.Range)WorkSheet.Cells[8, 7]).Value2 = Calc.c_mat;
                ((Microsoft.Office.Interop.Excel.Range)WorkSheet.Cells[9, 7]).Value2 = Calc.lamda_v;
                ((Microsoft.Office.Interop.Excel.Range)WorkSheet.Cells[10, 7]).Value2 = Calc.d_mash;


                // отобразить в журнале тестирования
                Console.WriteLine("--- Исходные данные");
                Console.WriteLine("Высота слоя, м: {0}", Calc.h_layer.ToString());
                Console.WriteLine("Начальная температура материала, С: {0}", Calc.t_beg_mat.ToString());
                Console.WriteLine("Начальная температура газа, С: {0}", Calc.t_beg_gaz.ToString());
                Console.WriteLine("Скорость газа на свободное сечение шахты, м/с: {0}", Calc.v_gaz.ToString());
                Console.WriteLine("Средняя теплоемкость газа, кДж/(м3*К): {0}", Calc.c_gaz.ToString());
                Console.WriteLine("Расход материалов, кг/с: {0}", Calc.mat_cons.ToString());
                Console.WriteLine("Теплоемкость материалов, кДж/(кг*К): {0}", Calc.c_mat.ToString());
                Console.WriteLine("Объемный коэффициент теплоотдачи, Вт/(м3*К): {0}", Calc.lamda_v.ToString());
                Console.WriteLine("Диаметр аппарата, м: {0}", Calc.d_mash.ToString());

                double FHR = double.Parse(((Excel.Range)WorkSheet.Cells[2, 16]).Value.ToString());
                double FRLHY0 = double.Parse(((Excel.Range)WorkSheet.Cells[3, 16]).Value.ToString());
                double FRLH = double.Parse(((Excel.Range)WorkSheet.Cells[4, 16]).Value.ToString());
               

                Assert.AreEqual(FHR, Math.Round(Calc.Flow_Heat_Ratio(), 3), 0.001);
                System.Diagnostics.Debug.WriteLine("Отношение теплоемкостей потоков: expected =" +
                            FHR + "; actual=" + Math.Round(Calc.Flow_Heat_Ratio(), 1));

                Assert.AreEqual(FRLHY0, Math.Round(Calc.Full_Relative_Layer_Height_Y0(), 3), 0.001);
                System.Diagnostics.Debug.WriteLine("Полная относительная высота слоя Y0: expected =" +
                            FRLHY0 + "; actual=" + Math.Round(Calc.Full_Relative_Layer_Height_Y0(), 3));

                Assert.AreEqual(FRLH, Math.Round(Calc.Full_Relative_Layer_Height(), 3), 0.001);
                System.Diagnostics.Debug.WriteLine("Полная относительная высота слоя: expected =" +
                            FRLH + "; actual=" + Math.Round(Calc.Full_Relative_Layer_Height(), 3));

               
                // отобразить в журнале тестирования
                Console.WriteLine("");
                Console.WriteLine("--- Результаты расчета");
                Console.WriteLine("Отношение теплоемкостей потоков, метод FHR(): expected = " +
                            FHR + "; actual=" + Math.Round(Calc.Flow_Heat_Ratio(), 1));

                // отобразить в журнале тестирования
                Console.WriteLine("");
                Console.WriteLine("--- Результаты расчета");
                Console.WriteLine("Полная относительная высота слоя Y0, метод FRLHY0(): expected = " +
                            FRLHY0 + "; actual=" + Math.Round(Calc.Full_Relative_Layer_Height_Y0(), 3));

                // отобразить в журнале тестирования
                Console.WriteLine("");
                Console.WriteLine("--- Результаты расчета");
                Console.WriteLine("Полная относительная высота слоя, метод FRLH(): expected = " +
                            FRLH + "; actual=" + Math.Round(Calc.Full_Relative_Layer_Height(), 3));

                
                WorkBook.Close(false, null, null);
                objExcel.Quit();

            }
            catch
            {
                if (WorkBook != null) WorkBook.Close(false, null, null);
                if (objExcel != null) objExcel.Quit();
            }
            finally
            {
                //WorkBook.Close(false, null, null);
                //objExcel.Quit();
            }
        }
    }
}
