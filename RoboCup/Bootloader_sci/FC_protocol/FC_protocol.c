/*
 * FC_Topology.c
 *
 *  Created on: Dec 12, 2011
 *      Author: B37531
 */
#include "common.h"
#include "uart.h"
#include "FC_protocol.h"
#include "flash.h"      /* include flash driver header file */

/********************************************************************/
////////////////////////////////////////////////////
// variable declare
uint16_t uiNumberCount = 0;
unsigned char g_ucFC_State;
static unsigned char m_uiRecCount;
static REC_FRAME_HEAD  m_RecFrame;
static unsigned char *m_pRecFrame;
static uint8_t  m_ucDataBuff[WRITE_BLOCK_SIZE];
const FC_IDENT_INFO m_MCU_Info = {
	FC_PROTOCOL_VERSION,			
	IDENT_SDID,	
	FLASH_NUM,
	USER_FLASH_START_ADDR,		
	USER_FLASH_END_ADDR,			
	RELOCATION_VERTOR_ADDR,		
	INTERRUPT_VERTOR_ADDR,		
	ERASE_BLOCK_SIZE,			
	WRITE_BLOCK_SIZE,			
	K15_STRING
};

#ifdef FLASH_LOCATION
#pragma location = ".flashConfig"
__root const uint32_t Config[] =
{
 0xFFFFFFFF,
 0xFFFFFFFF, 
 0xFFFFFFFF, 
 0xFFFEFFFF,
};
#endif

/**************************************************************/
void FC_Init( void );
void UART_Send(unsigned char *pData,uint16_t uiSize);
unsigned char FC_Communication( void );
void JumpToUserApplication(uint32_t userStartup);
void LONG_Convert(uint32_t *pLong);
uint16_t Flash_Write( uint32_t wNVMTargetAddress, uint8_t *pData, uint16_t sizeBytes);

/*****************************************************************************//*!
+FUNCTION----------------------------------------------------------------
* @function name: FC_Init
*
* @brief Initialize Flash  and valiable
*        
* @param  
* @return none
*
* @ Pass/ Fail criteria: none
*****************************************************************************/


void FC_Init( void )
{
	Flash_Init();
	m_pRecFrame = (uint8_t *)&m_RecFrame;
	g_ucFC_State = FC_STATE_NULL;
	m_uiRecCount = 0;
}
/*****************************************************************************//*!
+FUNCTION----------------------------------------------------------------
* @function name: UART_Send
*
* @brief send data by uart
*        
* @param  
*			pData - first address of transmitting data
*			uiSize - length
* @return none
*
* @ Pass/ Fail criteria: none
*****************************************************************************/

void UART_Send(uint8_t *pData,uint16_t uiSize)
{
	unsigned int i;
	for(i=0;i<uiSize;i++)
	{
		UART_putchar(TERM_PORT,pData[i]);
	}
}
	
/*****************************************************************************//*!
+FUNCTION----------------------------------------------------------------
* @function name: FC_Communication
*
* @brief hadle FC protocol with PC tools
*		 
* @param  
* @return none
*
* @ Pass/ Fail criteria: none
*****************************************************************************/

unsigned char FC_Communication( void )
{
	uint8_t uiReadData,i;
	uint8_t *pAddress;
	ADDRESS_TYPE * pTempAddress;
	if(UART_S1_RDRF_MASK != UART_getchar_present(TERM_PORT))
	{
		return 0;
	}

	// read data from uart
	uiReadData = UART_getchar(TERM_PORT);
	switch( g_ucFC_State )
	{
		case FC_STATE_NULL:
		{
			if( uiReadData == FC_CMD_ACK )
			{
				UART_putchar( TERM_PORT,0xfc );
				g_ucFC_State = FC_STATE_WORKING;
			}
			else
			{
				return 0;
			}
		}
		break;
		case FC_STATE_WORKING:
			{ 
				switch( uiReadData )
				{
				case FC_CMD_IDENT:
					{
						UART_putchar( TERM_PORT,m_MCU_Info.Version);
						UART_putchar( TERM_PORT,m_MCU_Info.Sdid>>8);
						UART_putchar( TERM_PORT,m_MCU_Info.Sdid);
						
						pTempAddress = (ADDRESS_TYPE *)&m_MCU_Info.BlocksCnt;
						for(i=0;i<7;i++)
						{
							UART_putchar( TERM_PORT,pTempAddress[i].Bytes.hh);
							UART_putchar( TERM_PORT,pTempAddress[i].Bytes.hl);
							UART_putchar( TERM_PORT,pTempAddress[i].Bytes.lh);
							UART_putchar( TERM_PORT,pTempAddress[i].Bytes.ll);
						}
						i = 0;
						do
						{
							UART_putchar( TERM_PORT,m_MCU_Info.IdString[i]);
						}while(m_MCU_Info.IdString[i++]);
									
				}
				break;
				case FC_CMD_ERASE:
					{
						g_ucFC_State = FC_STATE_EREASE;
				}
				break;
				case FC_CMD_WRITE:
					{
						g_ucFC_State = FC_STATE_WRITE_ADDRESS;
				}
				break;
				case FC_CMD_READ:
					{
						g_ucFC_State = FC_STATE_READ;
				}
				break;
				case FC_CMD_QUIT:
					{
						 SCB_VTOR = RELOCATION_VERTOR_ADDR;
						 JumpToUserApplication(RELOCATION_VERTOR_ADDR);   
                                                // SCB_VTOR = 0x1ffffc00;
                                                // JumpToUserApplication(0x1ffffc00);  
				}
				break;
				default:
					//UART_putchar( TERM_PORT,0xfc );
					break;
			}
			m_uiRecCount = 0;
		}
		break;
		
		case FC_STATE_EREASE:
			{
				m_pRecFrame[m_uiRecCount++] = uiReadData;
				if( m_uiRecCount >= sizeof(uint32_t) )
				{
					// erase
					LONG_Convert(&m_RecFrame.uiAddress);
					if(!Flash_EraseSector(m_RecFrame.uiAddress))
					{
						UART_putchar( TERM_PORT,FC_CMD_ACK );
					}
					else
					{
						UART_putchar( TERM_PORT,FC_CMD_NACK );
					}
					
					g_ucFC_State = FC_STATE_WORKING;
				}
		}
		break;
		case FC_STATE_WRITE_ADDRESS:
			{
				m_pRecFrame[m_uiRecCount++] = uiReadData;
				if( m_uiRecCount >= sizeof(uint32_t) )
				{
					g_ucFC_State = FC_STATE_WRITE_LEN;
				}
				
		}
		break;
		case FC_STATE_WRITE_LEN:
			{
				m_pRecFrame[m_uiRecCount++] = uiReadData;
                                g_ucFC_State = FC_STATE_WRITE_DATA;
		}
		break;
		case FC_STATE_WRITE_DATA:
			{
				m_pRecFrame[m_uiRecCount++] = uiReadData;
				if( m_uiRecCount > (m_RecFrame.Length + sizeof(uint32_t) ))
				{
					LONG_Convert(&m_RecFrame.uiAddress);
                    Memcpy_Byte((uint8_t *)&m_ucDataBuff[0],(uint8_t *)&m_RecFrame.DataBuff[0],m_RecFrame.Length);
			uiNumberCount ++;
                           
                    if( !Flash_Program(m_RecFrame.uiAddress,
				   (uint8_t *)&m_ucDataBuff[0],m_RecFrame.Length) )
					{
						UART_putchar( TERM_PORT,FC_CMD_ACK );
					}
					else
					{
						UART_putchar( TERM_PORT,FC_CMD_NACK );
					}
                  
					g_ucFC_State = FC_STATE_WORKING;
				}
			}	
			break;
		case FC_STATE_READ:
			{
				m_pRecFrame[m_uiRecCount++] = uiReadData;
				if( m_uiRecCount > sizeof(uint32_t) )
				{
					LONG_Convert(&m_RecFrame.uiAddress);
					pAddress = (uint8_t *)m_RecFrame.uiAddress;
					for( i=0;i<m_RecFrame.Length;i++)
					{
						UART_putchar( TERM_PORT,pAddress[i] );
					}
					g_ucFC_State = FC_STATE_WORKING;
				}
		}
		break;
		default:
			break;
		}
	return 1;
}


void JumpToUserApplication(uint32_t userStartup)
{
    /* set up stack pointer */  
    asm("LDR     r1, [r0]");
    asm("mov     r13, r1");
    /* jump to application reset vector */
    asm("ADDS      r0,r0,#0x04 ");
    asm("LDR      r0, [r0]");
    asm("BX       r0");
}

void LONG_Convert(uint32_t *pLong)
{
	unsigned char *p;
	unsigned char ucTemp;
	p = ( unsigned char *)pLong;
	ucTemp = p[0];
	p[0] = p[3];
	p[3] = ucTemp;
	ucTemp = p[1];
	p[1] = p[2];
	p[2] = ucTemp;
}
void Memcpy_Byte(uint8_t * Dest,uint8_t * Src,uint32_t Size)
{
	while(Size--)
	{
		*Dest++ = *Src++;
	}
}

uint16_t Flash_Write( uint32_t wNVMTargetAddress, uint8_t *pData, uint16_t sizeBytes)
{
	uint16_t err = FLASH_ERR_SUCCESS;
	uint8_t  wLeftBytes = (sizeBytes & 0x03);
	uint16_t wLeftLongWords = sizeBytes>>2;
	uint32_t wTargetAddress = wNVMTargetAddress;
	uint32_t dwData0,dwData1;
	uint32_t *pdwData = (uint32_t*)pData;
	int  i;
	
	// Check address to see if it is aligned to 4 bytes
	// Global address [1:0] must be 00.
	if(wNVMTargetAddress & 0x03)
	{
		err = FLASH_ERR_INVALID_PARAM;
		return (err);
	}
   	for(i = 0; i < wLeftLongWords; i++)
	{
		dwData0 = *pdwData++;
		err = Flash_Program1LongWord(wTargetAddress, dwData0);
		if(err)
		{			
			goto EndP;
			//break;
		}
		wTargetAddress += 4;
	}

	if(!wLeftBytes){
		return (err);
	}
        
#if     defined(BIG_ENDIAN)                
	dwData0 = 0;
	pData = (uint8_t*)pdwData;	// pointer to the left bytes
	for(i = wLeftBytes; i >0; i--)
	{
		dwData0 <<= 8;
		dwData0 |= *pData++;	// MSB byte first
	}
	// Calculate how many bytes need to be filled with 0xFFs
	// in order to form a single longword for the left bytes of data
	wLeftBytes = 4 - wLeftBytes;	
	//  
	for(i = wLeftBytes; i >0; i--)
	{
		dwData0 <<= 8;
		dwData0 |= 0xFF;	// MSB byte first
	}
#else
	dwData0 = 0xFFFFFFFFL;        
	pData = (uint8_t*)pdwData+wLeftBytes-1;	// pointer to the left bytes
	for(i = wLeftBytes; i >0; i--)
	{
		dwData0 <<= 8;
		dwData0 |= *pData--;	// MSB byte first
	}
#endif	
	// Now program the last longword
	err = Flash_Program1LongWord(wTargetAddress, dwData0);	
EndP:	
	return (err);
    
}
