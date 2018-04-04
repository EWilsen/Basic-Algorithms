/*                                                                                 
 * File:		plat_l1pt.c                                                            
 * Purpose:		Main process                                                         
 *                                                                                 
 */                                                                                
                                                                                   
#include "common.h"                                                                
#include "FC_protocol.h"                                                           
#include "uart.h"                                                                 
#include "sysinit.h"                                                               
#include "flash.h"                                                            
#define CONNECT_OVER_TIME 2                                                        
                                                                              
                                                   
/********************************************************************/             
int main(void)                                                                     
{                                                                                  
	uint32_t uiNullCounter = 0;                                                      
	uint32_t uiRepeatCount = 0;                                                      
	FC_Init();    
    for(;;) {	                                                                 
                                                              
 		if( !FC_Communication() )                                                      
		{                                                                              
			if( g_ucFC_State == FC_STATE_NULL )                                          
			{                                                                            
				uiNullCounter ++;                                                          
				if( uiNullCounter > 0xffff )                                               
				{                                                                          
					uiNullCounter = 0;                                                       
					uiRepeatCount ++;                                                        
					UART_putchar(TERM_PORT,0xfc);                                                     
				}                                                                          
				                                                                           
				if( uiRepeatCount > CONNECT_OVER_TIME )                                    
				{                                                                          
                    #ifdef FLASH_LOCATION                                          
					SCB_VTOR = RELOCATION_VERTOR_ADDR;                                       
					JumpToUserApplication(RELOCATION_VERTOR_ADDR);                           
                    #endif                                                         
				}                                                                          
			}                                                                            
		}                                                                              
                                                                                   
	}                                                                                
                                                                        
        return 0;                                                                  
}                                                                                  
 
                              
/********************************************************************/             
