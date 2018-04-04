/******************************************************************************
*
* Freescale Semiconductor Inc.
* (c) Copyright 2011-2012 Freescale Semiconductor, Inc.
* ALL RIGHTS RESERVED.
*
***************************************************************************//*!
*
* @file Hal_dev_mma.c
*
* @author 
*
* @version 0.0.1
*
* @date Aug 4, 2011
*
* @brief 
*
*******************************************************************************/
#include "common.h"
#include "i2c.h"

/******************************************************************************
* Global variables
******************************************************************************/

/******************************************************************************
* Constants and macros
******************************************************************************/
	
#define MMA8451_I2C_ADDRESS (0x1d<<1)
#define I2C0_B  I2C0_BASE_PTR

/******************************************************************************
* Local types
******************************************************************************/

/******************************************************************************
* Local function prototypes
******************************************************************************/

/******************************************************************************
* Local variables
******************************************************************************/

/******************************************************************************
* Local functions
******************************************************************************/

/******************************************************************************
* Global functions
******************************************************************************/
/*****************************************************************************//*!
   +FUNCTION----------------------------------------------------------------
   * @function name: HAl_DevMma8451Init
   *
   * @brief change clock from FEI mode to FBI mode and divide clock by 2
   *		
   * @param  none 
   *
   * @return none
   *
   * @ Pass/ Fail criteria: none
   *****************************************************************************/

void HAl_DevMma8451Init(void)
{
  I2C_Init(I2C0_B);
}

/*****************************************************************************//*!
   +FUNCTION----------------------------------------------------------------
   * @function name: pause
   *
   * @brief delay a few cycles
   *		
   * @param  none 
   *
   * @return none
   *
   * @ Pass/ Fail criteria: none
   *****************************************************************************/

static void pause(void)
{
    int n;
    for(n=0; n<40; n++);
      //  asm("nop");
}

/*****************************************************************************//*!
   +FUNCTION----------------------------------------------------------------
   * @function name: HAl_DevMma8451Init
   *
   * @brief read mma8451 register
   *		
   * @param  none 
   *
   * @return none
   *
   * @ Pass/ Fail criteria: none
   *****************************************************************************/

uint8 HAL_DevMma8451ReadReg(uint8 addr)
{
    uint8 result;

    I2C_start(I2C0_B);
    I2C_write_byte(I2C0_B, MMA8451_I2C_ADDRESS | I2C_WRITE);
    
    I2C_wait(I2C0_B);
    I2C_get_ack(I2C0_B);

    I2C_write_byte(I2C0_B, addr);
    I2C_wait(I2C0_B);
    I2C_get_ack(I2C0_B);

    I2C_repeated_start(I2C0_B);
    I2C_write_byte(I2C0_B, MMA8451_I2C_ADDRESS | I2C_READ);
    I2C_wait(I2C0_B);
    I2C_get_ack(I2C0_B);

    I2C_set_rx_mode(I2C0_B);

    I2C_give_nack(I2C0_B);
    result = I2C_read_byte(I2C0_B);
    I2C_wait(I2C0_B);

    I2C_stop(I2C0_B);
    result = I2C_read_byte(I2C0_B);
    pause();
    return result;
}
/*****************************************************************************//*!
   +FUNCTION----------------------------------------------------------------
   * @function name: HAL_DevMma8451WriteReg
   *
   * @brief write mma8451 register
   *		
   * @param  none 
   *
   * @return none
   *
   * @ Pass/ Fail criteria: none
   *****************************************************************************/
void HAL_DevMma8451WriteReg(uint8 addr, uint8 data)
{
    I2C_start(I2C0_B);

    I2C_write_byte(I2C0_B, MMA8451_I2C_ADDRESS|I2C_WRITE);
    I2C_wait(I2C0_B);
    I2C_get_ack(I2C0_B);

    I2C_write_byte(I2C0_B, addr);
    I2C_wait(I2C0_B);
    I2C_get_ack(I2C0_B);

    I2C_write_byte(I2C0_B, data);
    I2C_wait(I2C0_B);
    I2C_get_ack(I2C0_B);

    I2C_stop(I2C0_B);
    pause();
}





