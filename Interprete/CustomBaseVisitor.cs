using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interprete.SistemaExperto;

namespace Interprete.antlr
{
    /*-------------------------------------------------------------------------------------------
                 Hay 8 métodos a los cuales se les implementó diseño por contrato
                  -> Invariante de clase: Todos los métodos regresan un string
    /*-------------------------------------------------------------------------------------------*/

    public class CustomBaseVisitor : prologBaseVisitor<string>
    {
        public List<Clausula> listaHechos;
        public List<Regla> listaReglas;

        /*--------------------------------------------------------------------------------------------
                             Diseño por contrato aplicado a métodos constructores 
        ---------------------------------------------------------------------------------------------*/



        public CustomBaseVisitor()
        {
            listaReglas = new List<Regla>();
            listaHechos = new List<Clausula>();
        }
        /*--------------------------------------------------------------------------------------------
             1° CustomBaseVisitor
             ->  Def: listaReglas y listaHechos toman los valores nulos por defecto.
             ->  Pre: la clase contiene los atributos listaReglas y listaHechos.
             -> Post: listaReglas y listaHechos se inicializan con el valor vacío.
        ---------------------------------------------------------------------------------------------*/



        public override string VisitProlog([NotNull] prologParser.PrologContext context)
        {
            //Separamos las reglas cada vez que encuentre el caracter &.
            string[] reglas = context.l.Split("&");
            foreach (var rule in reglas)
            {
                Regla regla = new Regla();

                string[] clausulas = rule.Split("|");

                ///Caso 1 - Cabeza de la Regla.
                //clausula[0] cabeza de la regla. 
                string[] cabezaR = clausulas[0].Split("///");
                ///cabeza[0] nombre de la cabeza de la regla.
                ///cabeza[1] son los objetos de la cabeza de la regla.
                string[] objetosR = cabezaR[1].Split(",");
                regla.SetConsecuente(new Clausula(cabezaR[0], objetosR));

                ///Caso 2 - Cuerpo de la Regla.
                ///Hay que eliminar el elemento clausula[0]
                int indexToRemove = 0;
                string[] cuerpoR = clausulas.Where((source, index) => index != indexToRemove).ToArray();
                foreach (var h in cuerpoR)
                {
                    string[] cabezaH = h.Split("///");
                    string[] objetosH = cabezaH[1].Split(",");
                    regla.AddAntecedentes(new Clausula(cabezaH[0], objetosH));
                }
                listaReglas.Add(regla);
            }


            //Separamos los hechos cada vez que encuentre el caracter &.
            string[] hechos = context.r.Split("&");
            foreach (var h in hechos)
            {
                string[] claus = h.Split("///");
                string[] objetosClaus = claus[1].Split(",");
                Clausula clausula = new Clausula(claus[0], objetosClaus);
                listaHechos.Add(clausula);
            }

            return "completado";
        }
        /*--------------------------------------------------------------------------------------------
             2° VisitProlog
             ->  Pre: context no es nulo y es de tipo PrologContext.
             -> Post: regresa un string "completado", la lista de hechos almacenados en "listaHechos"
                      y la lista de reglas en "listaReglas".
        ---------------------------------------------------------------------------------------------*/



        public override string VisitDirective([NotNull] prologParser.DirectiveContext context)
        {
            var left = Visit(context.left);
            return (left + "|" + context.texto);
        }
        /*--------------------------------------------------------------------------------------------
             3° VisitDirectiva
             ->  Pre: context no es nulo y es de tipo DirectivaContext.
             -> Post: regresa un string con la pregunta leída de prolog. 
        ---------------------------------------------------------------------------------------------*/



        public override string VisitClause([NotNull] prologParser.ClauseContext context)
        {
            var left = Visit(context.left);
            string[] palabras = left.Split(' ');
            int indexToRemove = 0;
            string[] aux = palabras.Where((source, index) => index != indexToRemove).ToArray();
            listaHechos.Add(new Clausula(palabras[0], aux));
            return context.texto;
        }
        /*--------------------------------------------------------------------------------------------
             4° VisitClausula
             ->  Pre: context no es nulo y es de tipo ClausulaContext.
             -> Post: regresa un string context.texto y almacena un hecho leído de prolog en la lista
                      de hechos, listaHechos.
        ---------------------------------------------------------------------------------------------*/


        /*public override string VisitLista_term([NotNull] prologParser.List_termContext context)
        {
            return context.texto;
        }*/
        /*--------------------------------------------------------------------------------------------
             5° VisitLista_term
             ->  Pre: context no es nulo y es de tipo Lista_termContext.
             -> Post: regresa un string con la lista de términos leídos.
        ---------------------------------------------------------------------------------------------*/



        /*public override string VisitLista_termvar([NotNull] prologParser.TermlistContext context)
        {
            return context.texto;
        }*/
        /*--------------------------------------------------------------------------------------------
             6° VisitLista_termvar
             ->  Pre: context no es nulo y es de tipo Lista_termvarContext.
             -> Post: regresa un string con la lista de variables leídas.
        ---------------------------------------------------------------------------------------------*/



        public override string VisitCompound_term([NotNull] prologParser.Compound_termContext context)
        {
            var left = Visit(context.left);
            var right = Visit(context.right);
            return (left + " " + right);
        }
        /*--------------------------------------------------------------------------------------------
             7° VisitCompound_term
             ->  Pre: context no es nulo y es de tipo Compound_termContext.
             -> Post: regresa un string con la secuencia leída de la variable left y right con un espacio.
        ---------------------------------------------------------------------------------------------*/



        public override string VisitName([NotNull] prologParser.NameContext context)
        {
            string cadena = Convert.ToString(context.LETTER_DIGIT().GetText());
            return cadena;
        }
        /*--------------------------------------------------------------------------------------------
             8° VisitName
             ->  Pre: context no es nulo y es de tipo NameContext.
             -> Post: regresa un string con la secuencia leída del símbolo terminal LETTER_DIGIT
        ---------------------------------------------------------------------------------------------*/
    }
}