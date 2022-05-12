using System;

namespace Heat_Lib
{
    public class CalculationClass
    {
        #region ИСХОДНЫЕ ДАННЫЕ

        private double _h_layer;
        /// <summary>
        /// Высота слоя, м
        /// </summary>
        public double h_layer
        {
            get { return _h_layer; }
            set { _h_layer = value; }
        }

        private double _t_beg_mat;
        /// <summary>
        /// Начальная температура материала, ⁰С
        /// </summary>
        public double t_beg_mat
        {
            get { return _t_beg_mat; }
            set { _t_beg_mat = value; }
        }

        private double _t_beg_gaz;
        /// <summary>
        /// Начальная температура газа, ⁰С
        /// </summary>
        public double t_beg_gaz
        {
            get { return _t_beg_gaz; }
            set { _t_beg_gaz = value; }
        }

        private double _v_gaz;
        /// <summary>
        /// Скорость газа на свободное сечение шахты, м/с
        /// </summary>
        public double v_gaz
        {
            get { return _v_gaz; }
            set { _v_gaz = value; }
        }

        private double _c_gaz;
        /// <summary>
        /// Средняя теплоемкость газа, кДж/(м3*К)
        /// </summary>
        public double c_gaz
        {
            get { return _c_gaz; }
            set { _c_gaz = value; }
        }

        private double _mat_cons;
        /// <summary>
        /// Расход материалов, кг/с
        /// </summary>
        public double mat_cons
        {
            get { return _mat_cons; }
            set { _mat_cons = value; }
        }

        private double _c_mat;
        /// <summary>
        /// Теплоемкость материалов, кДж/(кг*К)
        /// </summary>
        public double c_mat
        {
            get { return _c_mat; }
            set { _c_mat = value; }
        }

        private double _lamda_v;
        /// <summary>
        /// Объемный коэффициент теплоотдачи, Вт/(м3*К)
        /// </summary>
        public double lamda_v
        {
            get { return _lamda_v; }
            set { _lamda_v = value; }
        }

        private double _d_mash;
        /// <summary>
        /// Диаметр аппарата, м
        /// </summary>
        public double d_mash
        {
            get { return _d_mash; }
            set { _d_mash = value; }
        }
        #endregion

        public static CalculationClass GetData()
        {
            return new CalculationClass
            {
                _h_layer = 6,
                _t_beg_mat = 650,
                _t_beg_gaz = 10,
                _v_gaz = 0.6,
                _c_gaz = 1.34,
                _mat_cons = 1.70,
                _c_mat = 1.49,
                _lamda_v = 2450,
                _d_mash = 2.10,
            };
        }

        public static CalculationClass calc { get; set; } = GetData();

        #region РАСЧЕТНЫЕ ПОКАЗАТЕЛИ

        private static double _FHR;
        /// <summary>
        /// Отношение теплоемкостей потоков 
        /// </summary>
        public double Flow_Heat_Ratio()
        {
            _FHR = (calc._c_mat * calc._mat_cons) / (calc._c_gaz * calc._v_gaz * 3.14 * Math.Pow((calc._d_mash / 2), 2));
            return _FHR;
        }

        private static double _FRLHY0;
        /// <summary>
        /// Полная относительная высота слоя Y0 
        /// </summary>
        public double Full_Relative_Layer_Height_Y0()
        {
            _FRLHY0 = (calc._lamda_v * calc._h_layer) / (calc._v_gaz * calc._c_gaz * 1000);
            return _FRLHY0;
        }

        private static double _FRLH;
        /// <summary>
        /// Полная относительная высота слоя  
        /// </summary>
        public double Full_Relative_Layer_Height()
        {
            _FRLH = 1 - _FHR * Math.Exp(((-(1 - _FHR)) * _FRLHY0) / _FHR);
            return _FRLH;
        }
        #endregion

        #region ОПРЕДЕЛЕНИЕ КООРДИНАТ

        public static double[] Param_Y()
        {
            double y = 0;
            double[] Param_Y = new double[13];
            for(int i = 0; i<Param_Y.Length; i++ )
            {
                Param_Y[i] = Math.Round((calc.lamda_v * y) / (calc.v_gaz * calc.c_gaz * 1000), 3);
                y += 0.5;
            }
            return Param_Y;
        }
        
        public static double[] Param_1exp()
        {
            double y = 0;
            double[] Param_1exp = new double[13];
            for (int i = 0; i < Param_1exp.Length; i++)
            {
                Param_1exp[i] = Math.Round(1 - Math.Exp((_FHR - 1) * Param_Y()[i] /_FHR), 3);
                y += 0.5;
            }
            return Param_1exp;

        }

        public static double[] Param_1mexp()
        {
            double y = 0;
            double[] Param_1mexp = new double[13];
            for (int i = 0; i < Param_1mexp.Length; i++)
            {
                Param_1mexp[i] = Math.Round(1 -_FHR * Math.Exp((_FHR - 1) * Param_Y()[i] / _FHR), 3);
                y += 0.5;
            }
            return Param_1mexp;

        }

        public static double[] Param_U()
        {
            double y = 0;
            double[] Param_U = new double[13];
            for (int i = 0; i < Param_U.Length; i++)
            {
                Param_U[i] = Math.Round(Param_1exp()[i] / (1 - (_FHR * Math.Exp((_FHR - 1) * _FRLHY0 / _FHR))), 3);
                y += 0.5;
            }
            return Param_U;

        }

        public static double[] Param_O()
        {
            double y = 0;
            double[] Param_O = new double[13];
            for (int i = 0; i < Param_O.Length; i++)
            {
                Param_O[i] = Math.Round(Param_1mexp()[i] / (1 - (_FHR * Math.Exp((_FHR - 1) * _FRLHY0 / _FHR))), 3);
                y += 0.5;
            }
            return Param_O;

        }

        public static double[] Param_t()
        {
            double y = 0;
            double[] Param_t = new double[13];
            for (int i = 0; i < Param_t.Length; i++)
            {
                Param_t[i] = Math.Round(calc.t_beg_gaz + (calc.t_beg_mat -calc.t_beg_gaz) * Param_U()[i], 3);
                y += 0.5;
            }
            return Param_t;

        }

        public static double[] Param_T()
        {
            double y = 0;
            double[] Param_T = new double[13];
            for (int i = 0; i < Param_T.Length; i++)
            {
                Param_T[i] = Math.Round(calc.t_beg_gaz + (calc.t_beg_mat - calc.t_beg_gaz) * Param_O()[i], 3);
                y += 0.5;
            }
            return Param_T;

        }

        public static double[] Param_Delta()
        {
            double y = 0;
            double[] Param_Delta = new double[13];
            for (int i = 0; i < Param_Delta.Length; i++)
            {
                Param_Delta[i] = Math.Round(Param_T()[i] - Param_t()[i], 3);
                y += 0.5;
            }
            return Param_Delta;

        }

        #endregion
    }
}
