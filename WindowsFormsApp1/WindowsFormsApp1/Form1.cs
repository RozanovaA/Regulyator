using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using Opc.Da;

//using System.Windows.Threading;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private double poloj_private;
        private double davl;
        private double ust;
        private int t=1;
        
        int KVO;
        int KVZ;

        public double Poloj
            {
            get
            {
                return poloj_private;
            }
            set
            {
                if (value > 100) { value = 100; }
                if (value < 0) { value = 0; }
                poloj_private = value;

                if (value > 95)
                {
                    KVO = 1;
                    textBox2.Text = "true";
                    textBox3.Clear();
                }
                else if (value < 5)
                {
                    KVZ = 1;
                    textBox3.Text = "true";
                    textBox2.Clear();
                }
                else
                {
                    KVZ = 0; KVO = 0;
                    textBox2.Clear();
                    textBox3.Clear();
                }

                //  
                
                textBox1.Text = Poloj.ToString();
            }
        }
        public double PIreg()
        {


            return 0;
        }

        public Form1()
        {
            InitializeComponent();
        }
        public static double integ;

        private Server server;
        private OpcCom.Factory fact = new OpcCom.Factory();

        private Subscription groupRead;
        private SubscriptionState groupState;

        private Subscription groupWrite;
        private SubscriptionState groupStateWrite;

        private List<Item> itemsList = new List<Item>();

        private void timer1_Tick(object sender, EventArgs e)
        {
            double P;
            try
            {
                davl = Convert.ToDouble(textBox5.Text);
                ust = Convert.ToDouble(textBox4.Text);

            }
            catch (Exception)
            {
                davl = 0;
                ust = 0;
            }
           
            P = ust - davl;
            double chast;
            if ((-0.2<P)&&(P<0.2))
            {
                integ = 0;
            }
            double a = Math.Abs(P);
            if ((a>0.2)&&(a<0.9))
            {
                integ=integ+P; 
                
                if (integ>1)
                {
                    Poloj -= P;
                    integ = 0;
                }
                if (integ < -1)
                {
                    Poloj -= P;
                    integ = 0;
                }

            }

            if ((P>0.9)||(P<-0.9))
            {
                chast = P * 0.5;
                Poloj -= chast;
            
            }         
            
     
            davl = (100- Poloj) / 20;
            textBox5.Text = davl.ToString();
            try
            {
                if (server is null)
                {
                    server = new Opc.Da.Server(fact, null);
                    server.Url = new Opc.URL("opcda://localhost/Intellution.OPCiFIX.1");

                    //2nd: Connect to the created server
                    server.Connect();

                    //Read group subscription
                    groupState = new Opc.Da.SubscriptionState();
                    groupState.Name = "myReadGroup";
                    groupState.UpdateRate = 200;
                    groupState.Active = true;
                    //Read group creation
                    groupRead = (Opc.Da.Subscription)server.CreateSubscription(groupState);
                    groupRead.DataChanged += new Opc.Da.DataChangedEventHandler(groupRead_DataChanged);

                    Item item = new Item();
                    item.ItemName = "FIX.DAVL.F_CV";
                    itemsList.Add(item);

                    item = new Item();
                    item.ItemName = "FIX.ISVO.F_CV";
                    itemsList.Add(item);

                    item = new Item();
                    item.ItemName = "FIX.ISVZ.F_CV";
                    itemsList.Add(item);

                    item = new Item();
                    item.ItemName = "FIX.UST.F_CV";
                    itemsList.Add(item);

                    item = new Item();
                    item.ItemName = "FIX.PROC_OTCR.F_CV";
                    itemsList.Add(item);

                    groupRead.AddItems(itemsList.ToArray());

                    groupStateWrite = new Opc.Da.SubscriptionState();
                    groupStateWrite.Name = "myWriteGroup";
                    groupStateWrite.Active = false;
                    groupWrite = (Opc.Da.Subscription)server.CreateSubscription(groupStateWrite);
                }
                groupWrite.RemoveItems(groupWrite.Items);
                List<Item> writeList = new List<Item>();
                List<ItemValue> valueList = new List<ItemValue>();

                Item itemToWrite = new Item();
                itemToWrite.ItemName = "FIX.DAVL.F_CV";
                ItemValue itemValue = new ItemValue(itemToWrite);
                itemValue.Value = davl;

                writeList.Add(itemToWrite);
                valueList.Add(itemValue);

                Item itemToWrite2 = new Item();
                itemToWrite2.ItemName = "FIX.UST.F_CV";
                ItemValue itemValue2 = new ItemValue(itemToWrite);
                itemValue2.Value = ust;

                writeList.Add(itemToWrite2);
                valueList.Add(itemValue2);

                Item itemToWrite3 = new Item();
                itemToWrite3.ItemName = "FIX.PROC_OTCR.F_CV";
                ItemValue itemValue3 = new ItemValue(itemToWrite);
                itemValue3.Value = Poloj;

                writeList.Add(itemToWrite3);
                valueList.Add(itemValue3);

                Item itemToWrite4 = new Item();
                itemToWrite4.ItemName = "FIX.ISVO.F_CV";
                ItemValue itemValue4 = new ItemValue(itemToWrite);
                itemValue4.Value = KVO;

                writeList.Add(itemToWrite4);
                valueList.Add(itemValue4);

                Item itemToWrite5 = new Item();
                itemToWrite5.ItemName = "FIX.ISVZ.F_CV";
                ItemValue itemValue5 = new ItemValue(itemToWrite);
                itemValue5.Value = KVZ;

                writeList.Add(itemToWrite5);
                valueList.Add(itemValue5);



                //IMPORTANT:
                //#1: assign the item to the group so the items gets a ServerHandle
                groupWrite.AddItems(writeList.ToArray());
                // #2: assign the server handle to the ItemValue
                for (int i = 0; i < valueList.Count; i++)
                    valueList[i].ServerHandle = groupWrite.Items[i].ServerHandle;
                // #3: write
                groupWrite.Write(valueList.ToArray());

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        void groupRead_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            foreach (ItemValueResult itemValue in values)
            {

                //MessageBox.Show(itemValue.Value.ToString());

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
