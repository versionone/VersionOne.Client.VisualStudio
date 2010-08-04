<project name='VersionOne.VisualStudio.VSPackage.Setup'>

	<property name='ver.major' value='8' />
	<property name='ver.minor' value='3' />
	<property name='ver.major.minor' value='${ver.major}.${ver.minor}' />
	<property name='build.version' value='${ver.major.minor}' />

	<if test="${property::exists('ver.build')}">
		<property name='build.version' value='${ver.major.minor}.${ver.build}' />
		<if test="${property::exists('ver.revision') and int::parse(ver.revision) > 0}">
			<property name='build.version' value='${ver.major.minor}.${ver.build}.${ver.revision}' />
		</if>
	</if>

</project>