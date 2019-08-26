
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bejeweled_multiplayer_client
{
    class conect
    {
        string txtMensajes;
        Stream stm;
        Form1 pad;
        bool Listen = true;
        public conect(Stream st, Form1 pa)
        {
            stm = st;
            pad = pa;
        }
        public void STOP()
        {
            Listen = false;
            stm.Close();
        }
        public void connect()
        {
            try
            {
                while (Listen)
                {
                    byte[] byteLectura = new byte[1024];
                    //stm.ReadTimeout=100;
                    int k = stm.Read(byteLectura, 0, 1024);
                    txtMensajes = "";
                    for (int i = 0; i < k; i++)
                    {
                        txtMensajes += Convert.ToChar(byteLectura[i]);
                    }
                    if (txtMensajes != "_X001D_")//disconect command
                    {

                        if (txtMensajes == "_X001O_")//offline comand
                        {
                            pad.msg("El Server se ha detenido");
                            pad.disc();
                        }
                        else
                        {
                            if (txtMensajes == "_X001Y_")//Conection acepted
                            {
                            }
                            else
                            {
                                if (txtMensajes == "_X001X_")
                                {//conection refused
                                    pad.msg("El Server se encuentra lleno");
                                }
                                else
                                {
                                    if (txtMensajes.Contains("WIN,"))
                                    {//win
                                        string[] temp = txtMensajes.Split(',');
                                        pad.msg(temp[1]+" es el ganador!!");
                                    }
                                    else
                                    {
                                        if (txtMensajes.Contains("REM,"))
                                        {//remove
                                            string[] temp = txtMensajes.Split(',');
                                            //pad.msg(temp[1] + " se ha retirado");
                                            pad.removeclient(temp[1]);
                                        }
                                        else
                                        {
                                            if (txtMensajes.Contains("CONN,"))
                                            {//nuevo cliente se conecta
                                                string[] temp = txtMensajes.Split(',');
                                                pad.addclient(temp[1]);
                                            }
                                            else
                                            {
                                                if (txtMensajes.Contains("_SCORE_:"))
                                                {
                                                    string[] temp = txtMensajes.Split(':');
                                                    pad.refresh_score(temp[1]);
                                                }
                                                else//any message
                                                {
                                                    //pad.msg(txtMensajes);
                                                    string[] temp=txtMensajes.Split('*');

                                                    pad.llenar(temp[0]);
                                                    pad.cambiar_score(temp[1], temp[2]);
                                                }
                                            }
                                            
                                        }
                                    }
                                    
                                }
                                
                            }
                            
                        }
                    }
                    else
                    {
                        pad.msg("Desconectado por inactividad");
                        pad.disc();
                    }
                }
            }
            catch (Exception ext)
            {
            }
        }
    }
}
