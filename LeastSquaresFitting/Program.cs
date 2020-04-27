using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.LinearAlgebra.Double;
namespace LeastSquaresFitting
{
    class Program
    {
        static double[] PointA = new double[ROW * COLUMN];
        static double[] PointB = new double[ROW * COLUMN];
        static double[] DiffPointA = new double[ROW * COLUMN];
        static double[] DiffPointB = new double[ROW * COLUMN];
        static double[] U = new double[COLUMN * COLUMN];
        static double[] A = new double[COLUMN * COLUMN];
        static List<double> H = new List<double>();
        static double meanA = 0.0f;
        static double meanB = 0.0f;
        const int ROW = 20;
        const int COLUMN = 3;

        static void Main(string[] args)
        {

            //1.从文档Point.txt中读入两个点集，存储在两个数组中;
            GetPointFromFile();
            //2.分别求两个点集的和的平均;
            meanA = GetMean(PointA);
            meanB = GetMean(PointB);
            //3.分别求两个点集与各自平均值的差，存入两个数组;
            MeanDifference(PointA, meanA, ref DiffPointA);
            MeanDifference(PointB, meanB, ref DiffPointB);
            //4.利用3,求出3*3的矩阵
            //MatrixMult为非通用矩阵相乘函数，仅限于本题
            MatrixMult(DiffPointA, DiffPointB);
            //5.对矩阵H进行奇异值分解。
            //求解SVD的过程有些复杂，这里使用了MathNet库提供的SVD函数。
            var m = DenseMatrix.OfArray(new double[,] {{H[0], H[1], H[2] }, { H[3], H[4], H[5] }, { H[6], H[7], H[8]}});
            var svd = m.Svd(true);
            //6.根据5的结果求出R,进而求出T
            var R = svd.U.Multiply(svd.VT);
            Console.WriteLine("R:");
            Console.Write(R);
            var T=R.Add(-meanB);
            T=R.Divide(meanA);
            Console.WriteLine("T:");
            Console.Write(T);

        }
        static void GetPointFromFile()
        {
            StreamReader sd = new StreamReader("Points.txt");
            String line = sd.ReadLine();
            int tempcout = 0;
            while ((line = sd.ReadLine()) != "_P")
            {
                string[] temp = line.Split(',');

                for (int i = 0; i < temp.Length; i++)
                {
                    PointA[tempcout++] = Convert.ToDouble(temp[i]);
                }
            }
            int tempcoutb = 0;
            while ((line = sd.ReadLine()) != "")
            {
                string[] temp = line.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    PointB[tempcoutb++] = Convert.ToDouble(temp[i]);
                }
            }
        }

        static double GetMean(double[] _Point)
        {
            double Sum = 0.0f;
            for (int i = 0; i < _Point.Length; i++)
            {
                Sum += _Point[i];
            }
            return Sum / _Point.Length;
        }
        static void MeanDifference(double[] _Point, double _mean, ref double[] _Pointout)
        {
            for (int i = 0; i < _Point.Length; i++)
            {
                _Pointout[i] = _Point[i] - _mean;
            }
        }
        static void MatrixMult(double[] _MatrixA, double[] _MatrixB)
        {
            int cout = 0, coutr = 0, coutc = 0;
            int coutwc =0, coutwr=0;
            double temp=0.0;
            while (coutwr < COLUMN)
            {
                while(coutwc < ROW*COLUMN)
                {
                    if (coutwc % 20!=0)
                    {                       
                        temp=temp+(DiffPointA[coutr+coutwr*20] * DiffPointB[coutc]);
                        coutwc++;
                        coutc += 3;
                        coutr++;
                    }
                    else
                    {
                        H.Add(temp);
                        coutc = 0;
                        cout++;
                        coutc += cout;                      
                        coutr = 0;
                        coutwc++;                        
                        temp = 0.0;
                    }
                }
                coutwr++;
                coutwc = 0;
                coutr = 0;
                coutc = 0;
                cout = 0;
            }
        }

    }
}
