/******************************************************************************
* @author a13984
*
* @version 0.0.1
*
* @date Jul-15-2011
*
* @brief application entry point which performs application specific tasks. 
*
*******************************************************************************
*
* provide a demo for how to initialize the PT60, output messages via SCI, 
* flash operations, etc.
* NOTE:
*	printf call may occupy a lot of memory (around 1924 bytes), so please
*	consider your code size before using printf.
******************************************************************************/
#include "common.h"
#include "ics.h"
#include "wdog.h"
#include "flash.h"
#include "uif.h"



/******************************************************************************
* External objects
******************************************************************************/
extern void cmd_eraseverify_flash_block(int argc, char **argv);
extern void cmd_eraseverify_eeprom_block(int argc, char **argv);
extern void cmd_eraseverify_flash_section(int argc, char **argv);
extern void cmd_eraseverify_eeprom_section(int argc, char **argv);
extern void cmd_eraseverify_all(int argc, char **argv);
extern void cmd_program_eeprom(int argc, char **argv);
extern void cmd_program_flash(int argc, char **argv);
extern void cmd_erase_eeprom_sector(int argc, char **argv);
extern void cmd_erase_flash_sector(int argc, char **argv);
extern void cmd_erase_eeprom_block(int argc, char **argv);
extern void cmd_erase_flash_block(int argc, char **argv);
extern void cmd_erase_all(int argc, char **argv);
extern void cmd_read_mem(int argc, char **argv);

/******************************************************************************
* Global variables
******************************************************************************/
uint8_t bDataBuf[512];	// must be aligned to 32-bit address boundary
uint8_t bDataSize;

/******************************************************************************
* Constants and macros
******************************************************************************/
#define MAX_NO_DATA				(UIF_MAX_ARGS-2)
#define FLASH_USER_PARAM_ADDR	0x7000
#define EEPROM_USER_PARAM_ADDR	0x10000000

const char BANNER0[] = "\n*********************************\n";
const char BANNER1[] = "\nWelcome to KE02Z Flash/EEPROM Lab!\n";
const char BANNER2[] = "\n*********************************\n";
const char BANNER3[] = "Please use recommended address to play with:\n" \
		               "  flash address = %#04.4x\n  EEPROM address = %#04.4x\n";
	
const char PROMPT[] = "CMD> ";
UIF_CMD UIF_CMDTAB[] =
{
	UIF_CMD_HELP
    {"ev_fb",0,1,0,cmd_eraseverify_flash_block, "EraseVerify flash block","<address in hex>"},
    {"ev_eep",0,1,0,cmd_eraseverify_eeprom_block,"EraseVerify EEPROM block","<address in hex>"},
    {"ev_fs",0,2,0,cmd_eraseverify_flash_section,"EraseVerify flash  section","<address in hex> <# of longwords>"},
    {"ev_eeps",0,2,0,cmd_eraseverify_eeprom_section, "EraseVerify EEPROM  section","<address in hex> <# of bytes>"},
    {"ev_all",0,0,0,cmd_eraseverify_all, "EraseVerify all blocks",""},
    {"pg_eep",0,MAX_NO_DATA+1,0,cmd_program_eeprom, "Program EEPROM","<address in hex> <data1> [... <data100>]"},
    {"pg_f",0,MAX_NO_DATA+1,0,cmd_program_flash, "Program flash","<address in hex> <data1> [... <data100>]"},
    {"ers_eeps",0,1,0,cmd_erase_eeprom_sector, "Erase EEPROM sector","<address in hex>"},
    {"ers_fs",0,1,0,cmd_erase_flash_sector, "Erase flash sector","<address in hex>"},
    {"ers_eep",0,1,0,cmd_erase_eeprom_block, "Erase EEPROM block","<address in hex>"},
    {"ers_fb",0,1,0,cmd_erase_flash_block, "Erase flash block","<address in hex>"},
    {"ers_all",0,0,0,cmd_erase_all, "Erase all blocks",""},
    {"read_mem",0,2,0,cmd_read_mem, "Read memory","<address in hex> <# of bytes>"},
};

UIF_SETCMD UIF_SETCMDTAB[] =
{
    {NULL, 0,1,NULL,""},
};

const int UIF_NUM_CMD    = UIF_CMDTAB_SIZE;
const int UIF_NUM_SETCMD = UIF_SETCMDTAB_SIZE;




/******************************************************************************
* Local types
******************************************************************************/

/******************************************************************************
* Local function prototypes
******************************************************************************/
void delay(void);

/******************************************************************************
* Local variables
******************************************************************************/

/******************************************************************************
* Local functions
******************************************************************************/
void simple_nvm_test(void);

/******************************************************************************
* Global functions
******************************************************************************/
int main(void) {
	int i;
	uint8_t *pUserData = (uint8_t*)bDataBuf;
  
  /* Initialize user LEDs */
  LED0_Init();
  LED1_Init();
  LED2_Init();
  
  /*
   * Call your routines
   */
   printf(BANNER0);
   printf(BANNER1);
   printf(BANNER2);
   printf(BANNER3,FLASH_USER_PARAM_ADDR,EEPROM_USER_PARAM_ADDR);
   uif_cmd_help(1, NULL);
   bDataSize = 100;
   for(i = 0; i < bDataSize; i++)
   {
	   pUserData[i] = 0x5A+i;   
   }
   Flash_Init();   
    
   /* Now it is safe to enable interrupts 
    *  
    */
  EnableInterrupts;
  for(;;) {
	    while (TRUE)
	    {
	        printf(PROMPT);
	        run_cmd();
	    }	
  } /* loop forever */
  /* please make sure that you never leave main */
  
  return 1;
}

void delay(void)
{
	int i,j;
	for(i = 0; i < 100; i++)
	{
		for(j = 0; j < 1600; j++)
		{
			//asm(nop);
			/* __RESET_WATCHDOG();	*/ /* feeds the dog */
		}
	}
}


void cmd_eraseverify_flash_block(int argc, char **argv)
{
	uint16_t wNVMTargetAddres;
	int success;
	int *pReturn= &success;
	uint16_t err;
    if (argc != 2)
	{
		printf("\nValid 'ev_fb' cmd syntax:\n");
		printf("CMD> ev_fb  <address in hex> \n\n");
		return;
	}
	else{	  
		wNVMTargetAddres = get_value(argv[1],pReturn,16);
		err = NVM_EraseVerifyBlock(wNVMTargetAddres,0);
		if(err)
		{
			printf("\nEraseVerify flash block failed (i.e.,flash is not blank)!\n");
		}
		else
		{
			printf("\nEraseVerify flash block success (flash is blank)!\n");
		}
	}
}


void cmd_eraseverify_eeprom_block(int argc, char **argv)
{
	uint16_t wNVMTargetAddres;
	int success;
	int *pReturn= &success;
	uint16_t err;
    if (argc != 2)
	{
		printf("\nValid 'ev_eep' cmd syntax:\n");
		printf("CMD> ev_eep  <address in hex> \n\n");
		return;
	}
	else{	  
		wNVMTargetAddres = get_value(argv[1],pReturn,16);
		err = NVM_EraseVerifyBlock(wNVMTargetAddres,1);
		if(err)
		{
			printf("\nEraseVerify EEPROM block failed (i.e.,EEPROM is not blank)!\n");
		}
		else
		{
			printf("\nEraseVerify EEPROM block success (EEPROM is blank)!\n");
		}
	}	
}

void cmd_eraseverify_flash_section(int argc, char **argv)
{
	uint16_t wNVMTargetAddres;
	uint16_t wLongwordCount;
	int success;
	int *pReturn= &success;
	uint16_t err;
	    
	if (argc != 3)
	{
		printf("\nValid 'ev_fs' cmd syntax:\n");
		printf("CMD> ev_fs  <address in hex> <# of longwords>\n\n");
		return;
	}
	else{	  
		wNVMTargetAddres = get_value(argv[1],pReturn,16);
		wLongwordCount = get_value(argv[2],pReturn,10);
		err = Flash_EraseVerifySection(wNVMTargetAddres,wLongwordCount);
		if(err)
		{
			printf("\nEraseVerify flash section failed (i.e.,flash section is not blank)!\n");
		}
		else
		{
			printf("\nEraseVerify flash section success (flash section is blank)!\n");
		}
	}		
}


void cmd_eraseverify_eeprom_section(int argc, char **argv)
{
	uint16_t wNVMTargetAddres;
	uint16_t wBytesCount;
	int success;
	int *pReturn= &success;
	uint16_t err;
		
	if (argc != 3)
	{
		printf("\nValid 'ev_eeps' cmd syntax:\n");
		printf("CMD>ev_eeps <address in hex> <# of bytes>\n\n");
		return;
	}
	else{	  
		wNVMTargetAddres = get_value(argv[1],pReturn,16);
		wBytesCount = get_value(argv[2],pReturn,10);
		err = EEPROM_EraseVerifySection(wNVMTargetAddres,wBytesCount);
		if(err)
		{
			printf("\nEraseVerify EEPROM section failed (i.e.,EEPROM section is not blank)!\n");
		}
		else
		{
			printf("\nEraseVerify EEPROM section success (EEPROM section is blank)!\n");
		}
	}		
}
void cmd_eraseverify_all(int argc, char **argv)
{
	uint16_t err;
	err = FTMRH_EraseVerifyAll();
	if(err)
	{
		printf("\nEraseVerify all blocks failed (i.e.,all blocks are not blank)!\n");
	}
	else
	{
		printf("\nEraseVerify all blocks success (all blocks are blank)!\n");
	}	 
}


void cmd_program_eeprom(int argc, char **argv)
{
	uint16_t wNVMTargetAddres;
	uint16_t wBytesCount;
	int success;
	int *pReturn= &success;
	uint8_t  bDataBuf[MAX_NO_DATA];
	uint16_t err;
	int i;
		
	if (argc < 3)
	{
		printf("\nValid 'pg_eep' cmd syntax:\n");
		printf("CMD>pg_eep <address in hex> <data1> <data2> ...\n\n");
		return;
	}else if (argc > (MAX_NO_DATA+2))
	{
		printf("\nError: too many data inputed, limit %d bytes!\n", MAX_NO_DATA);
	}
	else{	  
		wNVMTargetAddres = get_value(argv[1],pReturn,16);
		wBytesCount = argc-2;
		
		/* Get input data bytes */
		for(i = 0; i < wBytesCount; i++)
		{
			bDataBuf[i] = get_value(argv[2+i],pReturn,10);
		}
		err = EEPROM_Program(wNVMTargetAddres,bDataBuf,wBytesCount);
		if(err)
		{
			printf("\nProgram EEPROM failed!\n");
		}
		else
		{
			printf("\nProgram EEPROM  success!\n");
		}
	}			   
}


void cmd_program_flash(int argc, char **argv)
{
	uint16_t wNVMTargetAddres;
	uint16_t wBytesCount;
	int success;
	int *pReturn= &success;
	uint8_t  bDataBuf[MAX_NO_DATA];
	uint16_t err;
	int i;
		
	if (argc < 3)
	{
		printf("\nValid 'pg_flash' cmd syntax:\n");
		printf("CMD>pg_flash <address in hex> <data1> <data2> ...\n\n");
		return;
	}else if (argc > (MAX_NO_DATA+2))
	{
		printf("\nError: too many data inputed, limit %d bytes!\n", MAX_NO_DATA);
	}
	else{	  
		wNVMTargetAddres = get_value(argv[1],pReturn,16);
		wBytesCount = argc-2;
		
		/* Get input data bytes */
		for(i = 0; i < wBytesCount; i++)
		{
			bDataBuf[i] = get_value(argv[2+i],pReturn,10);
		}
		err = Flash_Program(wNVMTargetAddres,bDataBuf,wBytesCount);
		if(err)
		{
			printf("\nProgram flash failed!\n");
		}
		else
		{
			printf("\nProgram flash  success!\n");
		}
	}			       
}


void cmd_erase_eeprom_sector(int argc, char **argv)
{
	uint16_t wNVMTargetAddres;
	int success;
	int *pReturn= &success;
	uint16_t err;
		
	if (argc != 2)
	{
		printf("\nValid 'ers_eeps' cmd syntax:\n");
		printf("CMD> ers_eeps <address in hex>\n\n");
		return;
	}
	else{	  
		wNVMTargetAddres = get_value(argv[1],pReturn,16);
		err = EEPROM_EraseSector(wNVMTargetAddres);	
		if(err)
		{
			printf("\nErase EEPROM sector failed!\n");
		}
		else
		{
			printf("\nErase EEPROM sector success!\n");
		}	
	}	
}


void cmd_erase_flash_sector(int argc, char **argv)
{
	uint16_t wNVMTargetAddres;
	int success;
	int *pReturn= &success;
	uint16_t err;
		
	if (argc != 2)
	{
		printf("\nValid 'ers_fs' cmd syntax:\n");
		printf("CMD> ers_fs <address in hex>\n\n");
		return;
	}
	else{	  
		wNVMTargetAddres = get_value(argv[1],pReturn,16);
		err = Flash_EraseSector(wNVMTargetAddres);	
		if(err)
		{
			printf("\nErase flash sector failed!\n");
		}
		else
		{
			printf("\nErase flash sector success!\n");
		}	
	}	
}


void cmd_erase_eeprom_block(int argc, char **argv)
{
	uint16_t wNVMTargetAddres;
	int success;
	int *pReturn= &success;
	uint16_t err;
		
	if (argc != 2)
	{
		printf("\nValid 'ers_eep' cmd syntax:\n");
		printf("CMD> ers_eep <address in hex>\n\n");
		return;
	}
	else{	  
		wNVMTargetAddres = get_value(argv[1],pReturn,16);
		err = FTMRH_EraseBlock(wNVMTargetAddres,1);	
		if(err)
		{
			printf("\nErase EEPROM block failed!\n");
		}
		else
		{
			printf("\nErase EEPROM block success!\n");
		}	
	}
}

void cmd_erase_flash_block(int argc, char **argv)
{
	uint16_t wNVMTargetAddres;
	int success;
	int *pReturn= &success;
	uint16_t err;
		
	if (argc != 2)
	{
		printf("\nValid 'ers_fb' cmd syntax:\n");
		printf("CMD> ers_fb <address in hex>\n\n");
		return;
	}
	else{	  
		wNVMTargetAddres = get_value(argv[1],pReturn,16);
		err = FTMRH_EraseBlock(wNVMTargetAddres,0);	
		if(err)
		{
			printf("\nErase flash block failed!\n");
		}
		else
		{
			printf("\nErase flash block success!\n");
		}	
	}    
}
void cmd_erase_all(int argc, char **argv)
{
	uint16_t err;
	err = NVM_EraseAll();
	if(err)
	{
		printf("\nEraseAll blocks failed!\n");
	}
	else
	{
		printf("\nEraseAll blocks success!\n");
	}	 	
}
void cmd_read_mem(int argc, char **argv)
{
	uint16_t wNVMTargetAddres;
	uint16_t wLongwordCount;
	uint16_t i,j;
	int success;
	int *pReturn= &success;
	uint16_t err;
	    
	if (argc != 3)
	{
		printf("\nValid 'ev_fs' cmd syntax:\n");
		printf("CMD> ev_fs  <address in hex> <# of longwords>\n\n");
		return;
	}
	else{	  
		wNVMTargetAddres = get_value(argv[1],pReturn,16);
		wLongwordCount = get_value(argv[2],pReturn,10);
		printf("Memory address: 0x%x\n",wNVMTargetAddres);
		for(i=0;i<wLongwordCount;i++)
		{
			printf("0x%x,",*((uint8_t *)(i+wNVMTargetAddres)));
			if( ( i+1 )%8 == 0 )
			{
				printf("\n");
			}

		}
		printf("\n");
	}		
}

void simple_nvm_test(void)
{   
	uint16_t err;
	err = Flash_Program(FLASH_USER_PARAM_ADDR,
			   bDataBuf,bDataSize);
	if(err)
	{
		printf("\nFlash programming failed!\n");
	}
	err = EEPROM_Program(EEPROM_USER_PARAM_ADDR,bDataBuf,bDataSize);
	if(err)
	{
		printf("\nEEPROM programming failed!\n");
	}	
	err = EEPROM_EraseSector(EEPROM_USER_PARAM_ADDR);
	if(err)
	{
		printf("\nEEPROM erase sector  failed!\n");
	}	
	err = Flash_EraseSector(FLASH_USER_PARAM_ADDR);
	if(err)
	{
		printf("\nFlash erase sector failed!\n");
	}	
	err = FTMRH_EraseBlock(EEPROM_USER_PARAM_ADDR,1);
	if(err)
	{
		printf("\nEEPROM erase block failed!\n");
	}	
	err = NVM_EraseVerifyBlock(EEPROM_USER_PARAM_ADDR,1);
	if(err)
	{
		printf("\nEEPROM erase verify EEPROM block failed!\n");
	}	
	err = Flash_EraseVerifySection(FLASH_USER_PARAM_ADDR,16);
	if(err)
	{
		printf("\nErase verify flash section failed!\n");
	}	
	err = EEPROM_EraseVerifySection(EEPROM_USER_PARAM_ADDR,16);
	if(err)
	{
		printf("\nErase verify EEPROM section failed!\n");
	}	
	err = FTMRH_EraseVerifyAll();
	if(err)
	{
		printf("\nErase verify all blocks failed!\n");
	}	
	NVM_EraseAll();   	
}
