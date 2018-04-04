#include "common.h"
uint8_t Loop_TimerCount=0;
uint16 last_state[24];
uint16 next_state[24];
int main(void)
{
  uint16 k;
  /**************************************************************************/
  nj_gpio_Init();
  PIT_Init(0, PWM_PitModulo);
  /********修改复位值方法：用运行程序中的相同代码段替换以下内容**********/
  next_state[0]=703;
  next_state[1]=210;
  next_state[2]=210;
  next_state[3]=210;
  next_state[4]=210;
  next_state[5]=210;
  next_state[6]=210;
  next_state[7]=210;
  next_state[8]=210;
  next_state[9]=210;
  next_state[10]=210;
  next_state[11]=210;
  next_state[12]=210;
  next_state[13]=210;
  next_state[14]=210;
  next_state[15]=210;
  next_state[16]=210;
  next_state[17]=210;
  next_state[18]=210;
  next_state[19]=210;
  next_state[20]=210;
  next_state[21]=210;
  next_state[22]=210;
  next_state[23]=210;
  /********修改复位值方法：用运行程序中的相同代码段替换以上内容**********/
  for(k=0;k<24;k++)
  {
    last_state[k]=next_state[k];
  }
  /**********************仿人竞速传感器程序*************************/
   /*
  while(1)
  {
     if(PTA6_Logic)
     {
         
     }
     else if(PTA7_Logic)
     {
         
     }
     else if(PTF6_Logic)
     {
         
     }
     else if(PTF7_Logic)
     {
         
     }
     else if(PTH6_Logic)
     {
         
     }
     else if(PTH7_Logic)
     {
         
     }
     else 
     {
         
     }
  }
  */
  /*************************put your own code here!*********************/

 /****************code_is_over!*****************/
}
