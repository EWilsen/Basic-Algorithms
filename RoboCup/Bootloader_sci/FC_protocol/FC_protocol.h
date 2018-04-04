/*
 * FC_Topology.h
 *
 *  Created on: Dec 12, 2011
 *      Author: B37531
 */

#ifndef FC_TOPOLOGY_H_
#define FC_TOPOLOGY_H_


#define FC_STATE_NULL				0x00
#define FC_STATE_WORKING			0x01
#define FC_STATE_READ_IDENT			0x02
#define FC_STATE_EREASE				0x03
#define FC_STATE_WRITE_ADDRESS		        0x04
#define FC_STATE_WRITE_LEN			0x05
#define FC_STATE_WRITE_DATA			0x06
#define FC_STATE_READ				0x07


#define FC_CMD_ACK					0xfc
#define FC_CMD_NACK					0x03
#define FC_CMD_IDENT				0x49
#define FC_CMD_ERASE				0x45
#define FC_CMD_WRITE				0X57
#define FC_CMD_READ					0x52
#define FC_CMD_QUIT					0x51


#ifdef FLASH_LOCATION
#define ID_STRING_MAX						    5
#define K15_STRING							    "KE02"
#define FC_PROTOCOL_VERSION			            0x88
#define IDENT_SDID							    0x14a
#define USER_FLASH_START_ADDR		            0x1000
#define USER_FLASH_END_ADDR			            64*1024
#define RELOCATION_VERTOR_ADDR	                0x1000
#define INTERRUPT_VERTOR_ADDR		            0x0000
#define ERASE_BLOCK_SIZE				        512
#define WRITE_BLOCK_SIZE				        64
#define FLASH_NUM								1
#endif
#ifdef RAM_LOCATION
#define ID_STRING_MAX				        5
#define K15_STRING				            "KE02"
#define FC_PROTOCOL_VERSION			        0x88
#define IDENT_SDID				            0x0000
#define USER_FLASH_START_ADDR		        0x000
#define USER_FLASH_END_ADDR			        64*1024
#define RELOCATION_VERTOR_ADDR		        0x000
#define INTERRUPT_VERTOR_ADDR		        0x0000
#define ERASE_BLOCK_SIZE			        512
#define WRITE_BLOCK_SIZE			        64
#define FLASH_NUM				            1
#endif

typedef union Address 
{
	uint32_t complete;
	struct
	{
	  uint16_t low;
	  uint16_t high;        
	}Words;
	struct
	{
	  unsigned char ll;
	  unsigned char lh;
	  unsigned char hl;
	  unsigned char hh;
	}Bytes;
}ADDRESS_TYPE;

typedef uint32_t addrtype; 
  
typedef struct 
{
          /** version */
          unsigned char Version;
          /** Sd Id */
          uint16_t Sdid;
          /** count of flash blocks */
          addrtype BlocksCnt;
          /** flash blocks descritor */
          addrtype FlashStartAddress;
		  addrtype FlashEndAddress;
          /** Relocated interrupts vestor table */
		  addrtype RelocatedVectors;
          /** Interrupts vestor table */
		  addrtype InterruptsVectors;
          /** Erase Block Size */
		  addrtype EraseBlockSize;
          /** Write Block Size */
		  addrtype WriteBlockSize;
          /** Id string */
          char IdString[ID_STRING_MAX]; 	  
}FC_IDENT_INFO;



typedef struct
{
	uint32_t uiAddress;
	uint8_t  Length;
    uint8_t  DataBuff[WRITE_BLOCK_SIZE];   
}REC_FRAME_HEAD;


/*
globle variable
*/
extern unsigned char g_ucFC_State;

/*
globle function
*/

extern void FC_Init( void );
extern void UART_Send(uint8_t *pData,uint16_t uiSize);
extern unsigned char FC_Communication( void );
extern void JumpToUserApplication(uint32_t userStartup);
extern void LONG_Convert(uint32_t *pLong);
extern void Memcpy_Byte(uint8_t * Dest,uint8_t * src,uint32_t Size);



#endif /* FC_TOPOLOGY_H_ */
