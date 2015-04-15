using System;

namespace ZwaveExplorer
{
	public enum QueryStage
	{
		QueryStage_None,						/**< Query process hasn't started for this node */
		QueryStage_ProtocolInfo,				/**< Retrieve protocol information */
		QueryStage_Probe,						/**< Ping device to see if alive */
		QueryStage_WakeUp,						/**< Start wake up process if a sleeping node */
		QueryStage_ManufacturerSpecific1,		/**< Retrieve manufacturer name and product ids if ProtocolInfo lets us */
		QueryStage_NodeInfo,					/**< Retrieve info about supported, controlled command classes */
		QueryStage_ManufacturerSpecific2,		/**< Retrieve manufacturer name and product ids */
		QueryStage_Versions,					/**< Retrieve version information */
		QueryStage_Instances,					/**< Retrieve information about multiple command class instances */
		QueryStage_Static,						/**< Retrieve static information (doesn't change) */
		QueryStage_Probe1,						/**< Ping a device upon starting with configuration */
		QueryStage_Associations,				/**< Retrieve information about associations */
		QueryStage_Neighbors,					/**< Retrieve node neighbor list */
		QueryStage_Session,						/**< Retrieve session information (changes infrequently) */
		QueryStage_Dynamic,						/**< Retrieve dynamic information (changes frequently) */
		QueryStage_Configuration,				/**< Retrieve configurable parameter information (only done on request) */
		QueryStage_Complete,					/**< Query process is completed for this node */
	}
}

