# GNU Make workspace makefile autogenerated by Premake

.NOTPARALLEL:

ifndef config
  config=debug
endif

ifndef verbose
  SILENT = @
endif

ifeq ($(config),debug)
  slua_standalone_config = debug
  slua_standalone_tests_config = debug
endif
ifeq ($(config),release)
  slua_standalone_config = release
  slua_standalone_tests_config = release
endif

PROJECTS := slua-standalone slua-standalone-tests

.PHONY: all clean help $(PROJECTS) 

all: $(PROJECTS)

slua-standalone:
ifneq (,$(slua_standalone_config))
	@echo "==== Building slua-standalone ($(slua_standalone_config)) ===="
	@${MAKE} --no-print-directory -C . -f slua-standalone.make config=$(slua_standalone_config)
endif

slua-standalone-tests: slua-standalone
ifneq (,$(slua_standalone_tests_config))
	@echo "==== Building slua-standalone-tests ($(slua_standalone_tests_config)) ===="
	@${MAKE} --no-print-directory -C . -f slua-standalone-tests.make config=$(slua_standalone_tests_config)
endif

clean:
	@${MAKE} --no-print-directory -C . -f slua-standalone.make clean
	@${MAKE} --no-print-directory -C . -f slua-standalone-tests.make clean

help:
	@echo "Usage: make [config=name] [target]"
	@echo ""
	@echo "CONFIGURATIONS:"
	@echo "  debug"
	@echo "  release"
	@echo ""
	@echo "TARGETS:"
	@echo "   all (default)"
	@echo "   clean"
	@echo "   slua-standalone"
	@echo "   slua-standalone-tests"
	@echo ""
	@echo "For more information, see http://industriousone.com/premake/quick-start"